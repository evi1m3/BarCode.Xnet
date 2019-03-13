using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

using static BCx.SwissQrCodePayload;

namespace BCx.BarCodeWPFTest
{
    public class MainWindowVM
    {
        public  string      DataCode128     { get; private set; }   = "7560001000234";
        public  string      DataCodeQR      { get; private set; }   = "test for QR Code with some small data";
        public  string      DataAztec       { get; private set; }   = "test for Aztec Code with some small data";
        public  string      DataPdf417      { get; private set; }   = "Some barcode content to show any resulting pixels";

        public  Contact     DataCreditor    { get; private set; }   = new Contact("John Doe", "3003", "Bern", "CH", "Parlamentsgebäude", "1");
        public  Iban        DataIban        { get; private set; }   = new Iban("CH2609000000857666015", Iban.IbanType.Iban);
        public  Reference   DataReference   { get; private set; }   = new Reference(Reference.ReferenceType.QRR, "990005000000000320071012303", Reference.ReferenceTextType.QrReference);
        public  Currency    DataCurrency    { get; private set; }   = Currency.CHF;
        public  decimal     DataAmount      { get; private set; }   = 100.25m;
    }
}
