using WestWindLibrary.Entities;

namespace WestWindWeb.Components.Pages
{
    public partial class DataAnnotationExample
    {
        private PersonExample person = new();
        private string feedback = string.Empty;

        private void HandleValidSubmit()
        {
            feedback = "Submit all good!";
        }
    }
}
