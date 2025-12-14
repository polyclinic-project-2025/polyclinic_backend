using System.Text.Json;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicCore.Constants;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PolyclinicInfrastructure.Export
{
    public class PdfExportStrategy : IExportStrategy
    {
        public PdfExportStrategy()
        {
            // Configurar licencia de QuestPDF (Community License es gratuita)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Exporta datos a formato PDF
        /// </summary>
        /// <param name="data">Datos en formato JSON string o cualquier string</param>
        /// <param name="filePath">Ruta donde se guardará el PDF</param>
        public void Export(string data, string filePath, string name, List<string> columns)
        {
            // Intentar parsear los datos como JSON para obtener una estructura
            var dataObjects = ParseDataToObjects(data);

            // Generar el documento PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configuración de la página - usar orientación horizontal si hay muchas columnas
                    if (columns != null && columns.Count > 5)
                    {
                        page.Size(PageSizes.A4.Landscape());
                    }
                    else
                    {
                        page.Size(PageSizes.A4);
                    }
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    
                    // Ajustar tamaño de fuente según cantidad de columnas
                    var fontSize = columns != null && columns.Count > 6 ? 8 : (columns != null && columns.Count > 4 ? 9 : 10);
                    page.DefaultTextStyle(x => x.FontSize(fontSize).FontFamily("Arial"));

                    // Encabezado
                    page.Header().Element(c => ComposeHeader(c, name));

                    // Contenido principal
                    page.Content().Element(c => ComposeContent(c, dataObjects, columns));

                    // Pie de página
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf(filePath);
        }

        /// <summary>
        /// Compone el encabezado del PDF
        /// </summary>
        private void ComposeHeader(IContainer container, string name)
        {
            container.PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Reporte de Datos: {name} - Policlínico")
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);

                    column.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                });
            });
        }

        /// <summary>
        /// Compone el contenido principal del PDF con los datos
        /// </summary>
        private void ComposeContent(IContainer container, List<Dictionary<string, object>> dataObjects, List<string> selectedColumns = null)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(10);

                if (dataObjects == null || dataObjects.Count == 0)
                {
                    column.Item().Text("No hay datos para mostrar")
                        .FontSize(12)
                        .Italic()
                        .FontColor(Colors.Grey.Medium);
                    return;
                }

                // Usar las columnas seleccionadas si se proporcionan, de lo contrario usar todas las propiedades
                var properties = selectedColumns != null && selectedColumns.Count > 0
                    ? selectedColumns
                    : dataObjects.First().Keys.ToList();

                // Calcular el tamaño de fuente basado en la cantidad de columnas
                var columnCount = properties.Count;
                var cellFontSize = columnCount > 8 ? 7 : (columnCount > 6 ? 8 : (columnCount > 4 ? 9 : 10));
                var headerFontSize = cellFontSize;
                var cellPadding = columnCount > 6 ? 3 : 5;

                // Crear tabla con los datos
                column.Item().Table(table =>
                {
                    // Definir columnas según las propiedades con tamaño proporcional
                    table.ColumnsDefinition(columns =>
                    {
                        foreach (var _ in properties)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    // Encabezado de la tabla
                    table.Header(header =>
                    {
                        foreach (var property in properties)
                        {
                            // Obtener el nombre traducido del diccionario de PropertiesNameExport
                            var displayName = PropertiesNameExport.Properties.ContainsKey(property)
                                ? PropertiesNameExport.Properties[property]
                                : property;

                            header.Cell().Element(c => HeaderCellStyle(c, cellPadding))
                                .AlignCenter()
                                .AlignMiddle()
                                .Text(displayName)
                                .FontSize(headerFontSize)
                                .Bold()
                                .FontColor(Colors.White)
                                .WrapAnywhere();
                        }
                    });

                    // Filas de datos
                    foreach (var dataObject in dataObjects)
                    {
                        foreach (var property in properties)
                        {
                            var value = dataObject.ContainsKey(property) 
                                ? dataObject[property]?.ToString() ?? "N/A" 
                                : "N/A";

                            table.Cell().Element(c => DataCellStyle(c, cellPadding))
                                .AlignMiddle()
                                .Text(value)
                                .FontSize(cellFontSize)
                                .WrapAnywhere();
                        }
                    }
                });

                // Resumen al final
                column.Item().PaddingTop(10).Text($"Total de registros: {dataObjects.Count}")
                    .FontSize(11)
                    .Bold()
                    .FontColor(Colors.Blue.Darken1);
            });
        }

        /// <summary>
        /// Estilo para las celdas del encabezado
        /// </summary>
        private IContainer HeaderCellStyle(IContainer container, int padding)
        {
            return container
                .Background(Colors.Blue.Darken2)
                .Padding(padding)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1);
        }

        /// <summary>
        /// Estilo para las celdas de datos
        /// </summary>
        private IContainer DataCellStyle(IContainer container, int padding)
        {
            return container
                .Background(Colors.Grey.Lighten3)
                .Padding(padding)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1);
        }

        /// <summary>
        /// Compone el pie de página del PDF
        /// </summary>
        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Medium))
                .Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
        }

        /// <summary>
        /// Intenta parsear el string de datos a una lista de objetos (diccionarios)
        /// Soporta JSON Array o JSON Object
        /// </summary>
        private List<Dictionary<string, object>> ParseDataToObjects(string data)
        {
            try
            {
                // Intentar parsear como array de objetos JSON
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(data);

                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    // Es un array de objetos
                    return jsonElement.EnumerateArray()
                        .Select(item => JsonElementToDictionary(item))
                        .ToList();
                }
                else if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    // Es un solo objeto, convertirlo en lista con un elemento
                    return new List<Dictionary<string, object>> { JsonElementToDictionary(jsonElement) };
                }
            }
            catch (JsonException)
            {
                // Si no es JSON válido, crear un objeto simple con el texto
                return new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { { "Contenido", data } }
                };
            }

            return new List<Dictionary<string, object>>();
        }

        /// <summary>
        /// Convierte un JsonElement a un Dictionary<string, object>
        /// </summary>
        private Dictionary<string, object> JsonElementToDictionary(JsonElement element)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var property in element.EnumerateObject())
            {
                dictionary[property.Name] = GetJsonValue(property.Value);
            }

            return dictionary;
        }

        /// <summary>
        /// Obtiene el valor apropiado de un JsonElement según su tipo
        /// </summary>
        private object GetJsonValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => "NULL",
                JsonValueKind.Array => $"[{element.GetArrayLength()} elementos]",
                JsonValueKind.Object => "[Objeto]",
                _ => element.ToString()
            };
        }
    }
}
