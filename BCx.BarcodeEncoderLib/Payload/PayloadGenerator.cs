using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BCx
{
      public abstract class Payload
      {
         public abstract override string ToString();
      }

      public class TextPayload : Payload
      {
         string                        m_sText="";

         public TextPayload(string sText)
         {
            m_sText=sText;
         }

         public override string ToString()
         {
            return m_sText;
         }
      }

      public class SwissQrCodePayload : Payload
      {
         //Keep in mind, that the ECC level has to be set to "M" when generating a SwissQrCode!
         //SwissQrCode specification: https://www.paymentstandards.ch/dam/downloads/ig-qr-bill-de.pdf

         private readonly string br = "\r\n";
         private readonly string alternativeProcedure1, alternativeProcedure2;
         private readonly Iban iban;
         private readonly decimal? amount;
         private readonly Contact creditor, ultimateCreditor, debitor;
         private readonly Currency currency;
         private readonly DateTime? requestedDateOfPayment;
         private readonly Reference reference;

         /// <summary>
         /// Generates the payload for a SwissQrCode. (Don't forget to use ECC-Level M and set the Swiss flag icon to the final QR code.)
         /// </summary>
         /// <param name="iban">IBAN object</param>
         /// <param name="currency">Currency (either EUR or CHF)</param>
         /// <param name="creditor">Creditor (payee) information</param>
         /// <param name="reference">Reference information</param>
         /// <param name="debitor">Debitor (payer) information</param>
         /// <param name="amount">Amount</param>
         /// <param name="requestedDateOfPayment">Requested date of debitor's payment</param>
         /// <param name="ultimateCreditor">Ultimate creditor information (use only in consultation with your bank!)</param>
         /// <param name="alternativeProcedure1">Optional command for alternative processing mode - line 1</param>
         /// <param name="alternativeProcedure2">Optional command for alternative processing mode - line 2</param>
         public SwissQrCodePayload(Iban iban, Currency currency, Contact creditor, Reference reference, Contact debitor = null, decimal? amount = null, DateTime? requestedDateOfPayment = null, Contact ultimateCreditor = null, string alternativeProcedure1 = null, string alternativeProcedure2 = null)
         {
               this.iban = iban;

               this.creditor = creditor;
               this.ultimateCreditor = ultimateCreditor;

               if (amount != null && amount.ToString().Length > 12)
                  throw new SwissQrCodeException("Amount (including decimals) must be shorter than 13 places.");
               this.amount = amount;

               this.currency = currency;
               this.requestedDateOfPayment = requestedDateOfPayment;
               this.debitor = debitor;

               if (iban.IsQrIban && reference.RefType.Equals(Reference.ReferenceType.NON))
                  throw new SwissQrCodeException("If QR-IBAN is used, you have to choose \"QRR\" or \"SCOR\" as reference type!");
               this.reference = reference;

               if (alternativeProcedure1 != null && alternativeProcedure1.Length > 100)
                  throw new SwissQrCodeException("Alternative procedure information block 1 must be shorter than 101 chars.");
               this.alternativeProcedure1 = alternativeProcedure1;
               if (alternativeProcedure2 != null && alternativeProcedure2.Length > 100)
                  throw new SwissQrCodeException("Alternative procedure information block 2 must be shorter than 101 chars.");
               this.alternativeProcedure2 = alternativeProcedure2;
         }

         public class Reference
         {
               private readonly ReferenceType referenceType;
               private readonly string reference, unstructuredMessage;
               private readonly ReferenceTextType? referenceTextType;

               /// <summary>
               /// Creates a reference object which must be passed to the SwissQrCode instance
               /// </summary>
               /// <param name="referenceType">Type of the reference (QRR, SCOR or NON)</param>
               /// <param name="reference">Reference text</param>
               /// <param name="referenceTextType">Type of the reference text (QR-reference or Creditor Reference)</param>
               /// <param name="unstructuredMessage">Unstructured message</param>
               public Reference(ReferenceType referenceType, string reference = null, ReferenceTextType? referenceTextType = null, string unstructuredMessage = null)
               {
                  this.referenceType = referenceType;
                  this.referenceTextType = referenceTextType;

                  if (referenceType.Equals(ReferenceType.NON) && reference != null)
                     throw new SwissQrCodeReferenceException("Reference is only allowed when referenceType not equals \"NON\"");
                  if (!referenceType.Equals(ReferenceType.NON) && reference != null && referenceTextType == null)
                     throw new SwissQrCodeReferenceException("You have to set an ReferenceTextType when using the reference text.");
                  if (referenceTextType.Equals(ReferenceTextType.QrReference) && reference != null && (reference.Length > 27))
                     throw new SwissQrCodeReferenceException("QR-references have to be shorter than 28 chars.");
                  if (referenceTextType.Equals(ReferenceTextType.QrReference) && reference != null && !Regex.IsMatch(reference, @"^[0-9]+$"))
                     throw new SwissQrCodeReferenceException("QR-reference must exist out of digits only.");
                  if (referenceTextType.Equals(ReferenceTextType.QrReference) && reference != null && !ChecksumMod10(reference))
                     throw new SwissQrCodeReferenceException("QR-references is invalid. Checksum error.");
                  if (referenceTextType.Equals(ReferenceTextType.CreditorReferenceIso11649) && reference != null && (reference.Length > 25))
                     throw new SwissQrCodeReferenceException("Creditor references (ISO 11649) have to be shorter than 26 chars.");

                  this.reference = reference;

                  if (unstructuredMessage != null && (unstructuredMessage.Length > 140))
                     throw new SwissQrCodeReferenceException("The unstructured message must be shorter than 141 chars.");
                  this.unstructuredMessage = unstructuredMessage;
               }

               public ReferenceType RefType {
                  get { return referenceType; }
               }

               public string ReferenceText
               {
                  get { return !string.IsNullOrEmpty(reference) ? reference.Replace("\n", "") : null; }
               }

               public string UnstructureMessage
               {
                  get { return !string.IsNullOrEmpty(unstructuredMessage) ? unstructuredMessage.Replace("\n", "") : null; }
               }

               /// <summary>
               /// Reference type. When using a QR-IBAN you have to use either "QRR" or "SCOR"
               /// </summary>
               public enum ReferenceType
               {
                  QRR,
                  SCOR,
                  NON
               }

               public enum ReferenceTextType
               {
                  QrReference,
                  CreditorReferenceIso11649
               }

               public class SwissQrCodeReferenceException : Exception
               {
                  public SwissQrCodeReferenceException()
                  {
                  }

                  public SwissQrCodeReferenceException(string message)
                     : base(message)
                  {
                  }

                  public SwissQrCodeReferenceException(string message, Exception inner)
                     : base(message, inner)
                  {
                  }
               }
         }

         public class Iban
         {
               private string iban;
               private IbanType ibanType;

               /// <summary>
               /// IBAN object with type information
               /// </summary>
               /// <param name="iban">IBAN</param>
               /// <param name="ibanType">Type of IBAN (normal or QR-IBAN)</param>
               public Iban(string iban, IbanType ibanType)
               {
                  if (!IsValidIban(iban))
                     throw new SwissQrCodeIbanException("The IBAN entered isn't valid.");
                  if (!iban.StartsWith("CH") && !iban.StartsWith("LI"))
                     throw new SwissQrCodeIbanException("The IBAN must start with \"CH\" or \"LI\".");
                  this.iban = iban;
                  this.ibanType = ibanType;
               }

               public bool IsQrIban
               {
                  get { return ibanType.Equals(IbanType.QrIban); }
               }

               public override string ToString()
               {
                  return iban.Replace("\n", "").Replace(" ","");
               }

               public enum IbanType
               {
                  Iban,
                  QrIban
               }

               public class SwissQrCodeIbanException : Exception
               {
                  public SwissQrCodeIbanException()
                  {
                  }

                  public SwissQrCodeIbanException(string message)
                     : base(message)
                  {
                  }

                  public SwissQrCodeIbanException(string message, Exception inner)
                     : base(message, inner)
                  {
                  }
               }
         }

         public class Contact
         {
               private string br = "\r\n";
               private string name, street, houseNumber, zipCode, city, country;

               /// <summary>
               /// Contact type. Can be used for payee, ultimate payee, etc.
               /// </summary>
               /// <param name="name">Last name or company (optional first name)</param>
               /// <param name="zipCode">Zip-/Postcode</param>
               /// <param name="city">City name</param>
               /// <param name="country">Two-letter country code as defined in ISO 3166-1</param>
               /// <param name="street">Streetname without house number</param>
               /// <param name="houseNumber">House number</param>
               public Contact(string name, string zipCode, string city, string country, string street = null, string houseNumber = null)
               {
                  //Pattern extracted from https://qr-validation.iso-payments.ch as explained in https://github.com/codebude/QRCoder/issues/97
                  var charsetPattern = @"^([a-zA-Z0-9\.,;:'\ \-/\(\)?\*\[\]\{\}\\`´~ ]|[!""#%&<>÷=@_$£]|[àáâäçèéêëìíîïñòóôöùúûüýßÀÁÂÄÇÈÉÊËÌÍÎÏÒÓÔÖÙÚÛÜÑ])*$";

                  if (string.IsNullOrEmpty(name))
                     throw new SwissQrCodeContactException("Name must not be empty.");
                  if (name.Length > 70)
                     throw new SwissQrCodeContactException("Name must be shorter than 71 chars.");
                  if (!Regex.IsMatch(name, charsetPattern))
                     throw new SwissQrCodeContactException($"Name must match the following pattern as defined in pain.001: {charsetPattern}");
                  this.name = name;

                  if (!string.IsNullOrEmpty(street) && (street.Length > 70))
                     throw new SwissQrCodeContactException("Street must be shorter than 71 chars.");
                  if (!string.IsNullOrEmpty(street) && !Regex.IsMatch(street, charsetPattern))
                     throw new SwissQrCodeContactException($"Street must match the following pattern as defined in pain.001: {charsetPattern}");
                  this.street = street;

                  if (!string.IsNullOrEmpty(houseNumber) && houseNumber.Length > 16)
                     throw new SwissQrCodeContactException("House number must be shorter than 17 chars.");
                  this.houseNumber = houseNumber;

                  if (string.IsNullOrEmpty(zipCode))
                     throw new SwissQrCodeContactException("Zip code must not be empty.");
                  if (zipCode.Length > 16)
                     throw new SwissQrCodeContactException("Zip code must be shorter than 17 chars.");
                  if (!Regex.IsMatch(zipCode, charsetPattern))
                     throw new SwissQrCodeContactException($"Zip code must match the following pattern as defined in pain.001: {charsetPattern}");
                  this.zipCode = zipCode;

                  if (string.IsNullOrEmpty(city))
                     throw new SwissQrCodeContactException("City must not be empty.");
                  if (city.Length > 35)
                     throw new SwissQrCodeContactException("City name must be shorter than 36 chars.");
                  if (!Regex.IsMatch(city, charsetPattern))
                     throw new SwissQrCodeContactException($"City name must match the following pattern as defined in pain.001: {charsetPattern}");
                  this.city = city;

#if NET40
                  if (!CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(x => new RegionInfo(x.LCID).TwoLetterISORegionName.ToUpper() == country.ToUpper()).Any())
                     throw new SwissQrCodeContactException("Country must be a valid \"two letter\" country code as defined by  ISO 3166-1, but it isn't.");
#else
                  try { var cultureCheck = new CultureInfo(country.ToUpper()); }
                  catch { throw new SwissQrCodeContactException("Country must be a valid \"two letter\" country code as defined by  ISO 3166-1, but it isn't."); }
#endif

                  this.country = country;
               }

               public override string ToString()
               {
                  string contactData = name.Replace("\n", "") + br; //Name
                  contactData += (!string.IsNullOrEmpty(street) ? street.Replace("\n","") : string.Empty) + br; //StrtNm
                  contactData += (!string.IsNullOrEmpty(houseNumber) ? houseNumber.Replace("\n", "") : string.Empty) + br; //BldgNb
                  contactData += zipCode.Replace("\n", "") + br; //PstCd
                  contactData += city.Replace("\n", "") + br; //TwnNm
                  contactData += country + br; //Ctry
                  return contactData;
               }

               public class SwissQrCodeContactException : Exception
               {
                  public SwissQrCodeContactException()
                  {
                  }

                  public SwissQrCodeContactException(string message)
                     : base(message)
                  {
                  }

                  public SwissQrCodeContactException(string message, Exception inner)
                     : base(message, inner)
                  {
                  }
               }
         }

         public override string ToString()
         {
               //Header "logical" element
               var SwissQrCodePayload = "SPC" + br; //QRType
               SwissQrCodePayload += "0100" + br; //Version
               SwissQrCodePayload += "1" + br; //Coding

               //CdtrInf "logical" element
               SwissQrCodePayload += iban.ToString() + br; //IBAN


               //Cdtr "logical" element
               SwissQrCodePayload += creditor.ToString();

               //UltmtCdtr "logical" element
               if (ultimateCreditor != null)
                  SwissQrCodePayload += ultimateCreditor.ToString();
               else
                  SwissQrCodePayload += string.Concat(Enumerable.Repeat(br, 6).ToArray());

               //CcyAmtDate "logical" element
               SwissQrCodePayload += (amount != null ? $"{amount:0.00}" : string.Empty) + br; //Amt
               SwissQrCodePayload += currency + br; //Ccy
               SwissQrCodePayload += (requestedDateOfPayment != null ?  ((DateTime)requestedDateOfPayment).ToString("yyyy-MM-dd") : string.Empty) + br; //ReqdExctnDt

               //UltmtDbtr "logical" element
               if (debitor != null)
                  SwissQrCodePayload += debitor.ToString();
               else
                  SwissQrCodePayload += string.Concat(Enumerable.Repeat(br, 6).ToArray());


               //RmtInf "logical" element
               SwissQrCodePayload += reference.RefType.ToString() + br; //Tp
               SwissQrCodePayload += (!string.IsNullOrEmpty(reference.ReferenceText) ? reference.ReferenceText : string.Empty) + br; //Ref
               SwissQrCodePayload += (!string.IsNullOrEmpty(reference.UnstructureMessage) ? reference.UnstructureMessage : string.Empty) + br; //Ustrd

               //AltPmtInf "logical" element
               if (!string.IsNullOrEmpty(alternativeProcedure1))
                  SwissQrCodePayload += alternativeProcedure1.Replace("\n", "") + br; //AltPmt
               if (!string.IsNullOrEmpty(alternativeProcedure2))
                  SwissQrCodePayload += alternativeProcedure2.Replace("\n", "") + br; //AltPmt


               return SwissQrCodePayload;
         }


         /// <summary>
         /// ISO 4217 currency codes
         /// </summary>
         public enum Currency
         {
               CHF = 756,
               EUR = 978
         }

         public class SwissQrCodeException : Exception
         {
               public SwissQrCodeException()
               {
               }

               public SwissQrCodeException(string message)
                  : base(message)
               {
               }

               public SwissQrCodeException(string message, Exception inner)
                  : base(message, inner)
               {
               }
         }

         private static bool IsValidIban(string iban)
         {
            return Regex.IsMatch(iban.Replace(" ", ""), @"^[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}$");
         }

         public static bool ChecksumMod10(string digits)
         {
		      if (string.IsNullOrEmpty(digits) || digits.Length < 2)
                  return false;
            int[] mods = new int[] { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };

            int remainder = 0;
            for (int i = 0; i < digits.Length - 1; i++)
            {
                  var num = Convert.ToInt32(digits[i]) - 48;
                  remainder = mods[(num + remainder) % 10];
            }
            var checksum = (10 - remainder) % 10;
            return checksum == Convert.ToInt32(digits[digits.Length - 1]) - 48;
	      }

      }

}
