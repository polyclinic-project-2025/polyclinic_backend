#!/bin/bash
# Script para generar reporte de cobertura completo

echo "🧪 Ejecutando tests con cobertura..."

# Limpiar reportes anteriores
rm -rf TestResults
rm -rf coveragereport

# Ejecutar tests con cobertura usando el formato correcto
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Verificar si se generó el archivo
if [ ! -d "./TestResults" ]; then
    echo "❌ Error: No se generaron archivos de cobertura"
    exit 1
fi

echo ""
echo "📊 Generando reporte HTML..."

# Buscar el archivo de cobertura (puede estar en subdirectorios)
COVERAGE_FILE=$(find ./TestResults -name "coverage.cobertura.xml" -o -name "*.cobertura.xml" | head -1)

if [ -z "$COVERAGE_FILE" ]; then
    echo "❌ Error: No se encontró archivo de cobertura"
    echo "Archivos en TestResults:"
    find ./TestResults -type f
    exit 1
fi

echo "✅ Archivo de cobertura encontrado: $COVERAGE_FILE"

# Generar reporte visual
reportgenerator \
  -reports:"$COVERAGE_FILE" \
  -targetdir:"coveragereport" \
  -reporttypes:"Html;TextSummary"

if [ ! -f "coveragereport/Summary.txt" ]; then
    echo "❌ Error: No se generó el reporte"
    exit 1
fi

echo ""
echo "📈 Resumen de Cobertura:"
cat coveragereport/Summary.txt

echo ""
echo "✅ Reporte generado exitosamente en: coveragereport/index.html"
echo ""

# Abrir según el SO
if [[ "$OSTYPE" == "darwin"* ]]; then
    open coveragereport/index.html
else
    xdg-open coveragereport/index.html 2>/dev/null || echo "Abre manualmente: coveragereport/index.html"
fi