using System.Text.Json;
using PolyclinicApplication.Common.Interfaces;
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
        public void Export(string data, string filePath)
        {
            // Intentar parsear los datos como JSON para obtener una estructura
            var dataObjects = ParseDataToObjects(data);

            // Generar el documento PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configuración de la página
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Encabezado
                    page.Header().Element(ComposeHeader);

                    // Contenido principal
                    page.Content().Element(container => ComposeContent(container, dataObjects));

                    // Pie de página
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf(filePath);
        }

        /// <summary>
        /// Compone el encabezado del PDF
        /// </summary>
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Reporte de Datos - Policlínico")
                        .FontSize(20)
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
        private void ComposeContent(IContainer container, List<Dictionary<string, object>> dataObjects)
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

                // Obtener los nombres de las propiedades/columnas del primer objeto
                var properties = dataObjects.First().Keys.ToList();

                // Crear tabla con los datos
                column.Item().Table(table =>
                {
                    // Definir columnas según las propiedades
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
                            header.Cell().Element(CellStyle).Text(property)
                                .Bold()
                                .FontColor(Colors.White);
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Background(Colors.Blue.Darken2)
                                .Padding(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten1);
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

                            table.Cell().Element(CellStyle).Text(value);
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten1);
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
