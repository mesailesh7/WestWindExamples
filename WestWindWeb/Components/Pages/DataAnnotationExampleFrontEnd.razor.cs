using Microsoft.AspNetCore.Components.Forms;
using WestWindLibrary.Entities;

namespace WestWindWeb.Components.Pages
{
    public partial class DataAnnotationExampleFrontEnd
    {
        private PersonExample person = new();
        private string feedback = string.Empty;
        //Needed to add Validation Message when the user submits the form
        private EditContext editContext;
        private ValidationMessageStore validationMessageStore;
        private bool valid;

        protected override void OnInitialized()
        {
            //Add the instance of a class being validated to the EditContext
            //then pass the editContext to the Messages.
            editContext = new EditContext(person);
            validationMessageStore = new ValidationMessageStore(editContext);
            base.OnInitialized();
        }

        private void HandleSubmit()
        {
            feedback = string.Empty;
            //Clear the Validation Message Store to prep for rechecking
            validationMessageStore.Clear();

            //Validates the built in Data Annotation
            valid = editContext.Validate();

            //Get all fields we want to validate and add messages to
            var firstNameField = editContext.Field(nameof(person.FirstName));
            var lastNameField = editContext.Field(nameof(person.LastName));
            var birthdayField = editContext.Field(nameof(person.Birthday));
            
            //Business Rule: Persons cannot have the FirstName John when their LastName is Doe
            if(person.FirstName == "John" && person.LastName == "Doe")
            {
                validationMessageStore.Add(firstNameField, "John Doe is not allowed as a name.");
                validationMessageStore.Add(lastNameField, "John Doe is not allowed as a name.");
                valid = false;
            }
            //Business Rule: Users cannot be over 100 years old.
            if(person.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-100)))
            {
                validationMessageStore.Add(birthdayField, "You can't be over 100, geezer!");
                valid = false;
            }
            if(valid)
            {
                feedback = "Submit all good!";
            }
        }
        
        
        
    }
}
