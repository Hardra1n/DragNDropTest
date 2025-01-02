using System.Text;

namespace DragNDropTask
{
    public class LayoutSetting
    {
        private int _availableId = 1;


        public LayoutSetting(int rowCount, int columnCount)
        {
            if (rowCount <= 0 || columnCount <= 0)
            {
                throw new ArgumentException("Rows and columns counts must be greater then 0");
            }

            RowCount = rowCount;
            ColumnCount = columnCount;

            ElementsScheme = new int[RowCount, ColumnCount];
        }

        public int RowCount { get; init; }
        public int ColumnCount { get; init; }
        public int[,] ElementsScheme { get; private set; }

        public string Name { get; set; } = string.Empty;


        public bool AddElement(
            int topLeftRowIndex, 
            int topLeftColumnIndex, 
            int bottomRightRowIndex, 
            int bottomRightColumnIndex)
        {
            ThrowsIfNotValidIndex(topLeftRowIndex, topLeftColumnIndex);
            ThrowsIfNotValidIndex(bottomRightRowIndex, bottomRightColumnIndex);
            ThrowsIfIndexesNotValid(topLeftRowIndex, topLeftColumnIndex, bottomRightRowIndex, bottomRightColumnIndex);
            
            bool elementsSchemeHasFreeSpace = 
                CheckIfThereIsFreeSpace(topLeftRowIndex, topLeftColumnIndex, bottomRightRowIndex, bottomRightColumnIndex);
            if (!elementsSchemeHasFreeSpace)
            {
                return false;
            }

            int elementId = _availableId;
            _availableId++;

            for (int i = topLeftRowIndex; i <= bottomRightRowIndex; i++)
            {
                for (int j = topLeftColumnIndex; j <= bottomRightColumnIndex; j++)
                {
                    ElementsScheme[i, j] = elementId;
                }
            }

            return true;
        }

        public bool CheckIfThereIsFreeSpace(
            int topLeftRowIndex, 
            int topLeftColumnIndex, 
            int bottomRightRowIndex, 
            int bottomRightColumnIndex)
        {
            ThrowsIfNotValidIndex(topLeftRowIndex, topLeftColumnIndex);
            ThrowsIfNotValidIndex(bottomRightRowIndex, bottomRightColumnIndex);
            ThrowsIfIndexesNotValid(topLeftRowIndex, topLeftColumnIndex, bottomRightRowIndex, bottomRightColumnIndex);

            for (int i = topLeftRowIndex; i <= bottomRightRowIndex; i++)
            {
                for (int j = topLeftColumnIndex; j <= bottomRightColumnIndex; j++)
                {
                    if (ElementsScheme[i, j] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string SchemeToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    sb.AppendFormat(null, "{0, 3}", ElementsScheme[i, j]);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        private void ThrowsIfNotValidIndex(int rowIndex, int columnIndex)
        {

            if (rowIndex < 0 || rowIndex >= RowCount ||
                columnIndex < 0 || columnIndex >= ColumnCount)
            {
                throw new ArgumentException("Indexes must be not negative and less then count of dimension");
            }
        }

        private void ThrowsIfIndexesNotValid(
            int topLeftRowIndex, 
            int topLeftColumnIndex, 
            int bottomRightRowIndex,
            int bottomRightColumnIndex)
        {
            if (topLeftColumnIndex > bottomRightColumnIndex || topLeftRowIndex > bottomRightRowIndex)
            {
                throw new ArgumentException("Indexes later in dimensions mustn't be less");
            }
        }
    }
}
