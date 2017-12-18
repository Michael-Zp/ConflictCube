namespace ConflictCube.ComponentBased.Model.Components.Objects
{
    public enum InventoryItems
    {
        Cubes,
        Sledgehammer
    }

    public class PlayerInventory
    {
        public int Cubes;
        public InventoryItems SelectedItem = InventoryItems.Sledgehammer;

        public void MoveSelectedUp()
        {
            switch (SelectedItem)
            {
                case InventoryItems.Sledgehammer:
                    SelectedItem = InventoryItems.Cubes;
                    break;

                case InventoryItems.Cubes:
                    SelectedItem = InventoryItems.Sledgehammer;
                    break;
            }
        }

        public void MoveSelectedDown()
        {
            switch (SelectedItem)
            {
                case InventoryItems.Sledgehammer:
                    SelectedItem = InventoryItems.Cubes;
                    break;

                case InventoryItems.Cubes:
                    SelectedItem = InventoryItems.Sledgehammer;
                    break;
            }
        }

        public PlayerInventory(int cubes)
        {
            Cubes = cubes;
        }
    }
}
