using WestWindLibrary.Entities;
using Blazored.FluentValidation;

namespace WestWindWeb.Components.Pages
{
    public partial class FluentValidationExample
    {
        //When using FluentValidation you need to install two nuget packaged
        //FluentValidation
        //Blazored.FluentValidation
        //If using FluentValidation with a component Library you may not need a third-party Nuget Package
        //Example: MudBlazor does not need Blazored.FluentValidation, it understands FluentValidation built in.

        //Remember to register the validator from your class library in the Program.cs!!
            //Like a Service it needs to be registered to exist.

        private PersonExample person = new();
        private string feedback = string.Empty;
        //Used to validate within the EditForm
        private FluentValidationValidator? _fluentValidationValidator;

        private void HandleValidSubmit()
        {
        }
    }
}
