using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace PruebaAtlantidaBE.Shared
{
    public static class Utilidades
    {
        public static byte[] DescargarExcelDatatable(DataTable DT, string NombreExcel, string NombreHojaExcel, string Encabezado = "", string ColorEncabezado = "#000000", bool pesado = false)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(NombreHojaExcel);

            int FilaActual = 1;

            // Agrego el encabezado
            if (!string.IsNullOrEmpty(Encabezado))
            {
                wb.Worksheet(1).Range(wb.Worksheet(1).Cell(1, 1), wb.Worksheet(1).Cell(1, DT.Columns.Count)).Merge();
                wb.Worksheet(1).Cell(1, 1).Value = Encabezado;
                wb.Worksheet(1).Row(1).Style.Font.FontColor = XLColor.FromHtml(ColorEncabezado);
                FilaActual += 1;
            }


            if (DT.Rows.Count > 10000 & pesado == true)
            {
                DataTable dta;
                int inicio = 0;
                int Salto = 10000;

                // Agregamos el Header de las columnas
                for (int i = 0, loopTo = DT.Columns.Count - 1; i <= loopTo; i++)
                    wb.Worksheet(NombreHojaExcel).Cell(FilaActual, i + 1).Value = DT.Columns[i].ColumnName.ToString();

                // Agregamos estilo al header
                {
                    var withBlock = ws.Range(FilaActual, 1, FilaActual, DT.Columns.Count);
                    withBlock.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    withBlock.Style.Font.FontSize = 11;
                    withBlock.Style.Font.FontName = "Calibri";
                    withBlock.Style.Fill.BackgroundColor = XLColor.FromArgb(r: 184, g: 204, b: 228, a: 1);
                    withBlock.Style.Font.FontColor = XLColor.FromHtml("#222");
                    withBlock.Style.Font.Bold = true;
                }

                wb.Worksheet(NombreHojaExcel).Row(FilaActual).Style.Alignment.WrapText = true;
                FilaActual += 1;

                // las primeras 100 nada mas para ajustar las columnas xd
                dta = DT.AsEnumerable().Skip(inicio).Take(100).CopyToDataTable();
                wb.Worksheet(NombreHojaExcel).Cell(FilaActual, 1).InsertData(dta.Rows);
                wb.Worksheet(NombreHojaExcel).Columns().AdjustToContents();

                inicio += 100;
                FilaActual += 100;

                // Agregamos la data
                while (inicio < DT.Rows.Count)
                {
                    dta = DT.AsEnumerable().Skip(inicio).Take(Salto).CopyToDataTable();
                    wb.Worksheet(NombreHojaExcel).Cell(FilaActual, 1).InsertData(dta.Rows);

                    inicio += Salto;
                    FilaActual += Salto;
                }
                dta = new DataTable();
            }

            else
            {
                wb.Worksheet(NombreHojaExcel).Cell(FilaActual, 1).InsertTable(DT);
                wb.Worksheet(NombreHojaExcel).Columns().AdjustToContents();
                wb.Worksheet(NombreHojaExcel).Row(FilaActual).Style.Alignment.WrapText = true;
            }

            DT = new DataTable();

            // Guardar el libro de trabajo en un flujo de memoria
            using (var stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return stream.ToArray();
            }

        }

        public static DataTable ConvertirListaADataTable<T>(List<T> lista)
        {
            DataTable dataTable = new DataTable();

            // Obtener las propiedades del tipo T
            var propiedades = typeof(T).GetProperties();

            // Agregar las columnas al DataTable basadas en las propiedades del tipo T
            foreach (var propiedad in propiedades)
            {
                dataTable.Columns.Add(propiedad.Name, Nullable.GetUnderlyingType(propiedad.PropertyType) ?? propiedad.PropertyType);
            }

            // Agregar las filas al DataTable basadas en los elementos de la lista
            foreach (var item in lista)
            {
                DataRow row = dataTable.NewRow();
                foreach (var propiedad in propiedades)
                {
                    row[propiedad.Name] = propiedad.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }







    }
}
