using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
    [TestClass]
    public class CssParserTests
    {
        [TestMethod]
        public void AddStylesheet_ContainsAtCharsetRule_ShouldStripRuleAndParseStylesheet()
        {
            var stylesheet = "@charset utf-8; div { width: 100% }";

            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
        }

        [TestMethod]
        public void AddStylesheet_ContainsAtPageSection_ShouldStripRuleAndParseStylesheet()
        {
            var stylesheet = "@page :first { margin: 2in 3in; } div { width: 100% }";

            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);

            Assert.AreEqual(1, parser.Styles.Count);
            Assert.IsTrue(parser.Styles.ContainsKey("div"));
        }

        [TestMethod]
        public void AddStylesheet_ContainsUnsupportedMediaQuery_ShouldStrip()
        {
            var stylesheet = "@media print { div { width: 90%; } }";

            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);

            Assert.AreEqual(0, parser.Styles.Count);
        }

        [TestMethod]
        public void AddStylesheet_ContainsUnsupportedMediaQueryAndNormalRules_ShouldStripMediaQueryAndParseRules()
        {
            var stylesheet = "div { width: 600px; } @media only screen and (max-width:620px) { div { width: 100% } } p { font-family: serif; }";

            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);

            Assert.AreEqual(2, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);

            Assert.IsTrue(parser.Styles.ContainsKey("p"));
            Assert.AreEqual("serif", parser.Styles["p"].Attributes["font-family"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsSupportedMediaQuery_ShouldParseQueryRules()
        {
            var stylesheet = "@media only screen { div { width: 600px; } }";

            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);

            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsImportStatement_ShouldStripOutImportStatement()
        {
            var stylesheet = "@import url(http://google.com/stylesheet); div { width : 600px; }";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }


        [TestMethod]
        public void AddStylesheet_ContainsImportStatementTest_ShouldStripOutImportStatement()
        {
            var stylesheet = "@import 'stylesheet.css'; div { width : 600px; }";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsMinifiedImportStatement_ShouldStripOutImportStatement()
        {
            var stylesheet = "@import url(http://google.com/stylesheet);div{width:600px;}";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsMultipleImportStatement_ShouldStripOutImportStatements()
        {
            var stylesheet = "@import url(http://google.com/stylesheet); @import url(http://jquery.com/stylesheet1); @import url(http://amazon.com/stylesheet2); div { width : 600px; }";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsImportStatementWithMediaQuery_ShouldStripOutImportStatements()
        {
            var stylesheet = "@import url(http://google.com/stylesheet) mobile; div { width : 600px; }";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsMuiltpleImportStatementWithMediaQuerys_ShouldStripOutImportStatements()
        {
            var stylesheet = "@import url(http://google.com/stylesheet) mobile; @import url(http://google.com/stylesheet) mobile; @import url(http://google.com/stylesheet) mobile; div { width : 600px; }";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            Assert.AreEqual(1, parser.Styles.Count);

            Assert.IsTrue(parser.Styles.ContainsKey("div"));
            Assert.AreEqual("600px", parser.Styles["div"].Attributes["width"].Value);
        }

        [TestMethod]
        public void AddStylesheet_ContainsEncodedImage()
        {
            var stylesheet = @"#logo 
{ 
    content: url('data:image/jpeg; base64,/9j/4AAQSkZJRgABAgAAZABkAAD/7AARRHVja3kAAQAEAAAAPAAA/+4AJkFkb2JlAGTAAAAAAQMAFQQDBgoNAAAR5wAAFvUAACWwAAA0Bv/bAIQABgQEBAUEBgUFBgkGBQYJCwgGBggLDAoKCwoKDBAMDAwMDAwQDA4PEA8ODBMTFBQTExwbGxscHx8fHx8fHx8fHwEHBwcNDA0YEBAYGhURFRofHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8fHx8f/8IAEQgBaAJAAwERAAIRAQMRAf/EANMAAQEBAQEBAQEAAAAAAAAAAAAGBQQDBwIBAQEAAwEBAAAAAAAAAAAAAAAAAwQFAgEQAAEDAgYCAQQDAQAAAAAAAAABAgMTBBARMRQFFRI0MCBQMjNAoCEiEQABAgMEBwUIAgMBAAAAAAACAAERMQMhoRIyEEFRcYGRImGxQnITIDDwUoKyIzNAUKDB0eESAQACAgMAAAAAAAAAAAAAACFAUCCgYLAREwACAQIEBQQCAwEBAQAAAAAAAREhMfBRYXEQQZHR8YGhseEgwTBAUKBgcP/aAAwDAQACEQMRAAAB+qAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHjF1x1pQAAAAAAAAAAAAAAAAAAAAAAAAAAAO+3D++/AAAAAAM6lNP490AAAAAAAAAAAAAAAAAAAAAAAAAAACo3s/pn4AAAAAAzqU0/j3QAAAAAAAAAAAAAAAAAAAAAAAAAAAKjez+mfgAAAAADjrSYGPc8Y+xhW4se1EBtVJeCbiqz7GHaiw7kPvx1UZ88zfgo6M+Pai6Y+uSXngm4GrWkyrMYFdmWffn0CcuwZtjj349qM+xh24umP3o46mb8Htz7W5tjhm4m70P8ASkoT5FmLVrydMfU1fgpqE8toQalfvItRgVmbY6eOh7ScUu5R9peQAAAAAB/PE5iXuKtLxyc49qKZvwWeXa8OuZ29DV51iJ1atdm2OSTzfpywOzUqs6eR0q/0PFuRupW74O9SCTpj9x7Mc/dhr82zqV++WTnqi6/b2K1avpz7mz8UVGbNsR9sXeVYj6I+u6LrcqS/O9unU58/7597YupbRgoaU3ZF1Ea1W6yLUBs1L/Gt41mOdvQ2OZZ06/fp5722oqTaovQAAAAAAAy8+xhZVsZs8cVq1vo+HdxbUUzfg160nJJzaZdoD5xuUufvnaqS2uXahdeplzx7VWW0y7WVYjktKv8AQsW54deQOzT0YO7PLtRerVyLEf468uci3hXIe2Lvri6j9OtpQd7tOaW0a/0XEugQWxUoaU3ZF1Ea1W6yLUBs1PpmDeyrEcfp1vouJdA3tanp34AAAAAAAA8TmNd4akwzZ44rVrfR8O7i2opm/BT588bqVrPLs8cvOrWkh9erVZ1iP0630PEuR+pW0q/etXk0IO8yxxJaVf6Fi3BN3oOmPrbqyxWrV9+Pebvzo498+vO2Lvri6Efp1q3NsR2pWr8yz6ee/wB8YVyHqj66I+sK5FbZVmA2an0zBvZViOP0630XEugd9uGi2qQAAAAAAHJWkwsm3zQyAcUvEzfgs8u1mTx4luKuzbM7dhw7cPRx1VZ1iY0K9NQnxbUftz7zd858/AtMq1zd8z12GwzbIAEzfgy7Eftx7UZ9jGtRdMfXn15g3Ie2LuvzLGTZjnL0P9KWhP0ceyejX/Pqoz5+qPqR0q9vlWuCXidvQ2OZZAHTNHRbVL2l5AAAAAAzqU0/j3QAAAAAAAAAAAAAAAAAAAAAAAAAAAKjez+mfgAAAAADOpTT+PdAAAAAAAAAAAAAAAAAAAAAAAAAAAAqN7P6Z+AAAAAAOeHvgpzAAAAAAAAAAAAAAAAAAAAAAAAAAAAad6v6SeAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//aAAgBAQABBQL+mfLK2Ju/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+E38Jv4Tfwm/hN/Cb+EY9Ht+W+/T95tv0fLffp+8236Pluo3SRvhkZjyN5Lbr3F2dxdncXZLeStsLTkriW4LrlI4VXlrxVi5iZFguI545+VuWTWkrpbe+5J8U3H3NzcF7yM8Nx3F2dxdnI301vJ3F2dxdncXZA9Xw439/PBP3F2R8zLnFKyWPkL+a3m467luEvLyO2a7lrtVi5iZFjej2Xd9Fbj+XulVnL3SFpexXKXnJXENzYXD54LmR0cEHK3L5rh6xwM5a6V/I3s1u/uLs7i7O4uy3eskGDIZXkLVbF8yoil1DSeT2kE5ecfax20DUfP1VkclG2Ow473eSuFht4onyyR8RbNbccOhbwthiuvagnSDjGo+aWCFsMXK+7x1lbzW/VWRPZwTuvrC2itbWNslx1VkMajG3dy23ia5HNOY9vjLSCdl7DHDccNnQ5n2uE/HlVVb3jrW0ma/hm1J5Ehgc58j7fiI/Gbh4VSwtNvFyfvcP6l/wCna+1eepF+2e0gnW+sLaK1tmNfcdVZDGNYwtYKr9P4PIfrw5H0rX2jlvT473ebzOHy3WN17X/WXELGlycr7qPehxT3rdnJ+jYe4TNkWO5kuXP4917I45j22MlUVFReNmhfBzPtcJ+PIWFckjlidbcnPEvJPR9hZ5brHk/e4f1L/wBO19q89SL9pyfo2ft42H6v4N3Mkj8OR9K19o5b0+O93lIFlt4ZXRSRcpavbc8uxEs7pLiG69qCBJ+M/wC45LadJ4eV93jbu3it+xshFRU5P0bD3MF4t009lZSW0pzHt8XcwQs5S5hmk4VFq8z7XCfjfcht3JyNjKyWnVtYVl4tUcx1ty0LmTctbsTj76ucqxW3fGX0ULL/AJGB0NixX3d56kX7Tk/Rs/bxtJ0jf89zK6OOS4lk+i8jdJbQcbdsnOQhkmt7Pj7qO5LviWvVeNvUIuJuXLbWsVuyfjbt89nG6O25DjpZJuNt7uB1/YXMtz1V6dVekTVbFfRPltbXjrqO4+nkbK4mn6q9GcPcqttbR28fJWVxPPxltNAl5xdVy8beoW/ETK5qI1Lzjo7gfxd41WcXeOWz4+O3Lq1juI5OKu2qzi7xy2dky2bcsc+BnGXiPL6J8trbcddsn+iO4ljIXK+P5b79P3m2/R8t9+n7zbfo+WeKqzr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69Dr0OvQ69CNngz+md//aAAgBAgABBQL+me52RXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0K6FdCuhXQroV0EXP5p9PvMf4/LPp95j/H5ZW5orFTGNmZRQooUUEZ/wBPiREGxZlJBYUHNyEiTJ6ZKyLMkaiDI0VKKFFCNiKUUKKFFBdcY40VKKCwipkRxoqSMyGMzKKCwoKgxmYkKCwoPZkMjRUkbkrUzVYkyan+rChGzMooUUKKDk/3BGKoxMk+eVmSjXqgyRVV2lVSNf8AqTSNuaquQsyjZhy5jdFbm7QcuZFpI9UWqo16oMkVVcv+VVwY3PGHSV6oMXNJtYdJyLSRyoVhEzUdMJMpI/Mi/GbWPV2jNVGvVBkiqrtKqi4RMz/hXGmEertCHWTSAm0xboTaEWmRKn+EX5SaCDUQk8cIdMJEXOHScjkyEXMdEikaf9P0xi/GbWPV2jNVwi/J+mNvp/BlfmuEertCHWTSJ2SqmYsSjYR7chujnZO1HJksWkrFVaa4RflJphVyR788IdJWqpE1UJyHScZHmU3IIOXJ46ISFSRmREv+SszI41zev+M1XCL8n6YxPy/gSOyR0ir9DFyV0iZEa5K+RFQZKVUFmQc7MbImT1zWOT/JHIpHIiJVQqoKMXJXSJl9Mb0RKqCzIOdmRvREldmMlyKqDpsGSZFVBZUHyZjXZCSoVUHvzG6rKmDFyV0iZfQ2RUGLmnyz6feY/wAfln0+8x/j8r2+Rtzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Nubc25tzbm3Gpkn9M7/9oACAEDAAEFAv6Z6JmUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUymUxfmZr95dr8rNfvLtflaoi4uU8zzPMz/xHYK48jzEU8hBXDVFceZ5jlPM8zz+lzjzPPByjVFU8jzwVTyPMRRXDVFPIU8hynmeZ5iYqov8Bq54KgrRDxHaNHLh4CsEFM/8EHDUPEVBWiHjgq4vGoKMHjBw1DwxRh4DUHasFEFwVBWiHji5cv4UeLhMHDR4z6Fwbg7BuDtExUbng/Fo8YObgjh2ifQ7VgoguLtE+iT+ExMXCYOGjkw8hXiKKIn+CDhqnlg7RMfERMHjVHKMHjBXHkmCaCOPIao4ao5wguLtE+h6fwGoI36FEbg4RuCtPE8REFaIOaNQVDxPHBRG/U5DxPARByDUFaeIjcFaeJ4iNFQ8TxEQU8cFEb9KtF+Zmv3l2vys1+8u1+VFyKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKhUKgv9M//2gAIAQICBj8C1r/OGswui6ZhVOTFNbv/2gAIAQMCBj8C7Kj/2gAIAQEBBj8C/wAM/EUuxSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJSJMTSf33H+6Dd77j/dBu99AZxXUMNIenDqjGK8PJeHkvDyQ12hjeG5BTLDAp6MANjPXsZTZuC/ILE3J1jCSqA2GAk7NZsdBUKZTXp0oWZo7URHD02ss2p6YQg0Jrw8l4eSEQhB2javDyXh5Lw8lTN5kLO/FvYwBCEI2rw8l+QGduyxMYPEXTAEIOMbd6P1IdMIQVtpvlFWQHgvyCxN2WOhNpE0WUH6j1CumAsuqBMrLDaYo6Y4cLQnuWM5xhYjMZi1ipg+GBEzPZtdGbTFoshbptfYgYIWtrXh5Lw8l4eSA3mTRfT0jFCLzb38HVmV5aG9Ro4ZIzAeppWuqYvIiZn5rLe6wDlZ2gqXxqXTnOxk1McxLrib8lGgX0l/1DTHUq3nLvQ1H2Wb4qEzN0NMdSLh3LHUaJRhNZb3TFUaLtYjMBgTQhb2oALK72rLe6YWkLQZY3t1MyYmk9raPpZG9RowexEAPEWR7MVnJD5G73VXgijqhDkn9R/yfLGFiFwLoj1C6I9QtYycntIkz1rS+Vl+J3Au21l1fsLMqnD7WX1Oqu5UfOPeqvldBvZM9RowkjMBgTQhb2qmBZSeDrLe6YBkNjaLcoz/hDtjpq/GtUfOPfofeypfGpUdnV/pP5Xhd7Fbzl3rsXVmdujQXDuVhOyaJO9j6KnD7mVLfodqZYD1OsFd3cg1L06dSFMZ64aPpZdDE+5QJoP2rDTbDgmKHyN3uqvBYw/Y16gYuLqBv6gbHnzWMMpQVKMsTexU4fay+p1V3Kj5x71V8roN7aKnD7mVLzN7D7/4UGyjpq/GtUfOPfofeypfGpRHMFvDWhqDMV1PgfY6hQtL5nksXiaw27VW85d6Gm+trN8VsMHvZDUbXPei4dyw1Dwvikv296Z2k8lU4fcypb9NSpVLCzu8GafYjtxUyae7R9LI2qFhi9iH07cLWkqj6sKHyN3uqvBCAtiKZblCpZtEmii9PJHpXpv4meHOxQewmTNW6T26nX4/yFcnCp+xreCctRszt3J6VWy2LEnp0nxOU3VJm1FHlaqvldBvbRU4fcypeZvYgWUv4GIZxXU9mz2DAMzyVMnGwSZ3tbbowBaUWQGY9LTt0Y6L4X1jqX648WXXAG59ywhN5ltVQmGwid2tbagA8zTXqUWjizN2pxMfxl260RgMReGtZb2WW9kAvNmZnRgFpPCHNAZD0s9tre1jptFoQmst7LqdhbmsAcX2piptFsMJ9rqp6jQjCCepTKBvNnkv1x4so1ukdmtMzWM0libpqbdu9ZcXazq0cPa7rFmqfN/xYSsdspbF0tjba3/qy4e13XzVHmSqAMyZ2ZC7jJ9raDALSeEOapmQ2C8Xtb2YM9mxCTzf33H+6Dd77j/dBu99hjBZ7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5Z7lnuWe5MOz/DP/9oACAEBAwE/If8AjPiI3KMKHil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXc8Uu54pdzxS7nil3PFLueKXcmKglT/Nb2/v8A8QFvb+//ABAR3mB5F7kzuuq4r9KFBNo1WZodbuaHW7mh1u5zLmpzaCeZToTmzefB1hfqNXzJgk8kn7kTsxteD2FR82NO6eTEU1TNaDMg5ibha7RXzjWk1crqw+TWig031OyFeRmUG3VTmaHW7mh1u4imsoTWWs0aHW7mh1u5odbucgmbOT8FpdPQly51WRodbuLJn9zb3k5Uw9mKtjHhNWyzWQ57hIIvO4ktOVfl5InzqyUvmReXOb8Al5iSUcMT3GqflvkbGZJT8yPaOeTUP2GhTrH/ACnzQoOxQbdUeeo7R1+QoUEb1ZKwimiZrQZkM82S0oelRQnVze4kOLm4Tz3RodbuaHW7mh1u5DPFgtL432TOy6sp4oh/zoGiU7pid36M1wcu9pUNq+wxYkqUnNIrxqzRomanFqIiYSJc9w/kNcoZPlm+gp2WQu7FRZi5h6JEK4xQLPC1eb5sxDMXbEyXNmhCYucZt8y0QtXm+bPY/ENzMmUlRJZGpxajYaEIbVPQbEqyTuq/Yq2YSWNTi1KN6iNEoRAaUrMz9DsZWmaPhYxVY0mylBtctCkWF6w2rF+7yE8KHu/3EzYq7YP5bKy8oWUXWZJjS52NGiHfuiyXUeS3perYs7b6tkJaTdjm+SE32HLlx17LhZcxUXAcQyGLZGBZjybLEG18DYlWSd1X7EISijRs1OLUUpCoNFwUx57VkhJIklCVl/Rh0npx9gvgYhk4y9w/lwAvV+sn9PwxDMOjM5chtV1T68/WOHsfiFsKMk2hOKLTbf42DoY3TXrMjy2DUop0oLjV7aSI+VcxKElM6ljFViNxivFv4H9jmKhiXN0Oq6855zwoe7/chp6S4h2TLchMdpp0YuMc2w27hrZLHnRsw9Oae/42XMVFwHEMhi2RgWfGzFs/wjq9q/otpKXYW6tBPN83x9gvgYhk4y9w/kQmlsVngku80rXNeomNvPU30akfVzcqE2TqxUt2j2MxDMXLGN8kaGJyq+sJhyKFOSXR7H4h3UlOSdoWSNB07B7UoltH+Ng84s7wzOyoK6NCtctRNer4WMVWLNsho08tDmbigameVcjIQie7dPjhQ93+4lJWV8sm7G+i5p0QmOzVamg8hJpup737hhQ3armmhwMVHFvUpYdZPkJJr1N/oesRUkUT0bD7XA2XYNoi6FKrycbFp54GklfnByYB9n7DFsjAs+NmLZ/g4ULjyef9CkygqiKNkovwTtKVC3NMUHTs5CRvg0pqETFtxSxNcoPk1wY0H1bftkPI9RdwTtTbcuncRgzK4UHbs5DZoTtCXC/NsgYgvJUc65iHV6YOE5+ohMhBwVlHM0OLU0OLU94BhIQFZlrK/wBCrSlNJ+SejDk0VU3maHFqK2dnMui7l8E1fdhA1Ks2irJz3EspnoNO05Cr7W/Pe6Hkeou4IUK7q5bpKQmuFCZJDCf2+x+yCJUyX7hkaQuS/UsqZ67lsMxYRdh6h5bEvaBBGqZL9Sx8c4MloIYlUatC2MIb0HvwQFZlrK/0JYJVnIT/ABTerKoskEuP5re39/8AiAt7f3/4gEybkzc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYbmuw3Ndhua7Dc12G5rsNzXYblR5jE/8Z/8A/9oACAECAwE/If8AjPWss3zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN83zfN8jT/N8v8AtWv5vl/2rX8zIkXhcVTk3TdN0Q8gkHB1TohHyQx4Y8jKIKSWW25OGbpui2pum6bosMvwkTN0TyY9oZMmKjAzaIeWIHA3bxJbtJQxESIhCzMkREHmLWpum6bpAy42hDEp/wA7UmxPhYRETGhnpw5pF82hCklnIU4KVJb2KSNpNESp4cYXDsIqpkTPhty5JcDUcPlGNB8jPiPnOQU26wnitykCSSGSOeqSdCwfCWS/sWdy0sIqpjwz4bS54S62Ql/Rsb8bZf24t85j5/wt7cEfFtBXDYLvB1NaoUU8wiVarw+UbXMTFquZ85yFi7CLC0UZFBl3b8LB8JZL+xZ3LeFgu7fhd3/pWqy42y/txb56gLSGJ6jhLLexUSiaMpnDoA0xosF3ioCQtdeHyjxQJKliPnOQjyyZQnFblcLj08g5ehfqxNtGtKEpMics7lvCwXdvwn1s/wCg+ZFz/BCGxtlpwTIxkS4Q0CzRW1R71E0WghjQlQCulyAPj7yxaWx9l+UYfD5Ae8sjDzExggQ7CzRGobkbTyE0TGbR7jeg0jdo8IyDwWlsfZfjYyAf83y/7Vr+b5f9q1/NGghmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZkMyGZDMhmQzIZlE/4z//2gAIAQMDAT8h/wCNCSJEiRIkSJEiRIkSJEiRIkSJEiRIkSJEiRIkSJEiRIkSJEiRIkSJEiRIkSJEiRIkSJEhIf8A8eiFJiXxaiRIkO8Ob4QkxMSDeR5RExjHJkiQ1EiRIVvwYmSEE5GpkpGTEwmRjcTE45MkQ0ITyNQTjUSJEhqcUoeX/QjcEMUkXERBcQISkSGQJBcKAuJBeJaIiWKSEqR4QcbhLFhlhdwXiGMNwi5mDQh4VhaXFohLFJCVIiXCN/Su42F3C0u4Lvwu4X8LyR68a/gxnz4lwuDKC7gkGoGoYX/jWFpcWi41/wCF39KFcbC7haXEyE4EpkEyLiQWGlF4lIhxr+NbqRvhcKQplxdwREgxJ4E8xqSi1EoRAtS0XGv/AAlX9CZiF+Cyhk8ElDk+GSSE5AMkWESOgxD2yZMQsoZP5NbJiYgGtjkTkhvPhMNxOREg3E5GJQT8FlDJ/FDFh/8Ax6Inf7QAAAAAAAAAaX/xn//aAAwDAQACEQMRAAAQkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkklAAAAAAAAAAAAAAAAAAAAAAAAAAAADkkkkkklAAAAAAAAAAAAAAAAAAAAAAAAAAAAFkkkkkklAAAAAAAAAAAAAAAAAAAAAAAAAAAAFkkkkkklYEAGoxDuqAGAGADBEeUZgTqtTAFA5kkkkkkk6CtHoV1FVKFyBPAk1FyouYEFBwFEckkkkkkkmAIFtYABYPYoGrAxVGVxAAEFAtADkkkkkkkk1AIFvTLEiPLoAuAutES9QxKlAtAGkkkkkkknYAJFoliJgZM/AAAL12sdBBorw4AAfkkkkkklAAAAAAAAAAAAAAAAAAAAAAAAAAAAFkkkkkklAAAAAAAAAAAAAAAAAAAAAAAAAAAAFkkkkkkk22222222222222222222222222223kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk/9oACAEBAwE/EP8AjPSHIkUTcuXzeT/GBgwYMGDBgwYMGDBgwYMGDBgwYMGDBgwYMGDBgwYMGDB/1AYMGDBgwYMGDBgwYMGDBgwYNFqQiCcaw3/NjOX+09u+X/NjOX+09u+X/MiqLgNqhJ83uJ5sC33BcXxKclVOiiPwBEiRQ5arZ1WnEp5ZixBQy0FSNtzXC3MLJwpdIqyyXUem1Z8l7/uLjl4velSDkckdC6TaM6jEuWA5CS+aELERQLElSJt8kQOhpTmIRYXKypVo4cJuhc3Q+RcEoW1UnN8SJFr5Vm3JIyPwIkSMGShhCgNCyl/g+hZ9vkEdVwBFLSCIJWdSv2IUwlrNNUaOTTuOjJI2kVVQhRZSuYVCs5l5BqqZIhuLsqDTJdFqNXMckBxFrzbaBSaJJaBKlCnKnJw43UPfQemk6541by6IR32sM3SXRijUtxaVoFJPA7kqtUfLScwuhUjI4BlRt5im2cUsylXSaEJcMBwGh80MntJ5JgSpVChzbSQhMu5JzmlQpH4BEiRjtJ5IgS4VeKmbBt9gQoyiqadZzX86CC0ISmnyaYsQotOZfBPBBZEpVQm5ZD+PjFE3aNxZiL4Rm1KwlaPgYc0d82yUvm68JjJfEejRqV7ULVlwUBZK7ZkkpY0GLfybFr1bGneXOyU9hU2a9SqFYcN9W7vgOgMb98k9ar0J7au125bfLORNrLbVm74GFgQsicgqMlz4GJsL2Hk3ZlzZV3BUlN0basw5YrWbZqHzVeBhF8ozbhZEvRDVKZ1DdqxsjYm8tOzRKfTiRbsaodOXMhsZBoZNMN2shb1n/QPbgLwLItGfrUKkuxbpyKBpSrVO2QjVyVirVxlM2qvUbFKuwn0KWSJkwy7Ysl7JCOcqpPzJUWjgTiKrjdHNO8+hMpGfckkrLyV93xEI+5/K4DsRzGJZRuxjodNzzIq7gqSm6NtWYeCQ02m4Dqq8DDQSVpttIhVdeElECQqSfYqLrJQhQklkv6KTX2Ep/X5TB2B5vhMeh8l8xyXzXour8R2oK6atOQoGsGyuXda98+Jip45hk7JkhSsMsuTfET2P4fBWqCWTS5IlDKeEBkziWkiqUVE2mTf1NpErrVCHNGdJJtEt5uIXAi5GtDqk9YC8Htmic06jNUF5Um7SqzWOAvAsi6amaCdUz5JyfUlSKYXZzsPdFB8UzKs3VplRsQQe05qNT6wYyYP0fkIR9z+VwHYjmMSy8RMRyfgs9c+dqX7/AKLAxJFLbokkNBquSZ7FIX5TB2B5vhMf6DpVwQi9PYMrSiDsjozRHDI65tQ9CjXvoMLKQnJPOEzdJbiNRCQyLpeovrgOjITt889aj0F5PRv6Q0RTpxJ0npO2nAw8CU2LkhOplwmRZuziJTrpwE9j+HxdnbIjFcMyaBUr6DjI1qoVhvKsp9OJF9F4CUoTQxqvC9YE0kE2oPlzHVE5h5QX7G4C8CyL3WabOOTJWuLJcqjOUxT1MKvTmOhOpZU6Xr1K0hMppm9IJj4h0yjP5TGBV1KguSLbPmogXTiqQnoV7GP4ZuFJvkznG0aiiXkIpuupZJeqNQJOA2sjgcfQSMjpqpJuIoKZmb8ko0+wxHMYll4iYjk/Bi1EuQi2ytRNNJpynVNfzqyGahyoab5NZDw7O6+pF/X8FcFxjSNq8dLITOozmnhCeS4IXsVZEhq1ZIbx8Q8k7RObvg399Rkd2yTe2Gth7lTk9T+XU04oodGNdUGJpwOK3K0xZLkkNnUZTbwhvJiuC4hpE3eKlmQi1Yi0Lis9xPsmSPW0E5hKP0EcCGNSlqMndcTBhXsKyZhFOq1QnfUVpHN1cKzClu1HEofJOfyWRLzHIOjJ8+BjOS8yVoiH0COhs3Vq/wBIcPdhxYUMuSDyqlapJXPMT4akG80knoNbD3KnJ6n8uo9RtKpbJOoZz6CSqJWyEJLZD5S8NFMVlmaPcaE24UC/R/aEzmfA9H9ouUk01Qld1Uje5XA00EsXpzT5oRyI6mRqxuki1LcNCl6P7QsEBD1EK8fKXUWSak0k4Cq6C5l3zUQ3wJ31FaRzdXCswsnWnNSHRP8AFVba2WkbTb0Ia7MQp0/mxnL/AGnt3y/5sZy/2nt3y/5ktlUsS5E1ESszwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQeCDwQKURwHUT6f8Z/8A/9oACAECAwE/EP8AjPrsieRp9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu5p9K7mn0ruafSu4pK2f83xv3/tex/m+N+/9r2P80dZcl7lxX1KGI/oxH9GI/ofWmAypMrgv7gJqzfqOKpp9eBB2JlpMQ1LIrFrYglKXwKckmI/oxH9DS5UZiP6MR/RiP6FqLJtfhXBMmI/ofVZPUYLiGCSZj4HdSsj2lEuxRVl+owq2n1GOa6KuqZhJVljVpQzrVuYvzS5+TlggSws2KxMpNinFmxBnkzGRyozEf0Yj+jEf0KUWT43+a9i+Al/OhIdUW7S7cJCdx5FHoNQXTfBq+yHTrtMwtxdfMGmwh92IMsnqhr25ntXwOemddoJHZAx7cyz6/JLikGr7ISQ9B4FHPLRjll0jV9kPY12MhDmad1w+Z+hAT3QhoqRhmL2FkXev9CEsc5+R6o7iYSaGh6szI0qJD0KFmKOmHuWmyx8z5Z8T9nuj3r4Pai7sxA09x4FHPLRjVF0jV9kNc13wV667CEoVv6MNbs44Wx718cPhZhbkfb+yUNn7/D2r4FE6jtNprws+vyMXSEKErrh8z4Z7LgijUIYUC2qb3GfM/QvcluIalWJ085i9hZF3r/RrD7CaWTQgn9AdzxSTo5vw+Z8s+J+z3R718HtRd2fD5nwz3r8IanYv6LcC4O444Wx718cPhZhbioXajsMNhjiigNOaFkOh5cj2r4GPTP2gazCBrmwiz6/JJUlQYUDE4d0fM+Gey4pU5aVZ9xXpCPh8z9DEWaDlbPIZRc5L2FkXev9ExQXLcch1plDlVFweUfFRNJKqmSyoOuQUXk/YpK1dxOr0sKC4SFMPKOtD2ou7Ph8z4Z71+CmrsBP+fnMkSQ1MlT8LZCEgdW3J5cOWyB5FXpwasErPmM/RiC5iWdA0Dqk5PItkMm1awsNqNOQisrXkavszV9mIa1ZtloBT8MUx1ay/KXFZNX2YmrnwAOza9iGUtpIxcpkM/Ri6ip7DGl3ZWFcbHPo3Qio52RTrZO5Ol6oWVchdedkPck5CXFkxtknyyfC0Ap+GJQ6tZP8UkNTJ1GtbtfzfG/f+17H+b437/2vY/zJluKyYq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+zFX2Yq+yKnMf8Z//2gAIAQMDAT8Q/wCM9rQjVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9DVWPQ1Vj0NVY9BrE+X83xf7V7+b4v8AavfzTt5FjfGJg2TZNkYk5hIT4KoVWM1L1ELKFWQ1DY+BFVdiII2TZGsI2TZNkeUf4QJGyN5oWkohSHTkQuo3ilyRSJ3jdqCN6ibbjQkMmY5jQ6iIGY2+Q9UGybJskiPjd2Ka1/Om1VF8uuF1GRoSUWpoilBFktF2PaEIXrwCUhF/dkWxJtqxKQi6ShmiMZY6tCmiZoiUKBayJyp4fCKXIuBF7cs7FrGrF1biJo6Ew8httIiA5ahBrcuHyl8t7ou7F6LiOrQpomaIhKFwtl2Nz/Rubcbpb3XHsnKfD+F/dlR1ffhdEyGPhuFvgrao4Y445BmcJ04fCI3Yai49BcizsWsuVcY1S8VRNJFnf8Lh8pfLe6LuxeuFws7/AIWdv6VWd3xulvdceyWDkPaUOqtBQqxf3ZBoq2qK4XSINmqJyXC3xbMbDnZcPhEjljRQXizsWsmwiJUjLixTRppjyzC1qlm7kU8xZDGIImUu7F64XCzv+DZldf0ExMs6r+DGJCaOOfBsCERvgt1Dyx+9BawhtnHMYhMbIHcOxOEaJoiwkh7UhVG/ylCNEZvQWsImSyJWSRKuPLGnyCUCK7MR5DPKBFfMWsMTtUZ5CV1HNkhF24PakLo2vxq7VRLUv5vi/wBq9/N8X+1e/mrESTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8ieRPInkTyJ5E8iUef/Gf/AP/Z'); 
    max-width: 200px; 
    height: auto; 
}";
            var parser = new CssParser();
            parser.AddStyleSheet(stylesheet);
            var attributes = parser.Styles["#logo"].Attributes;
        }
    }
}