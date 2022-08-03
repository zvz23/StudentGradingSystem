using backend.Models;
using backend.Forms;

namespace backend.Forms.Helpers {

    public class StudentAccountHelper {

        public static StudentAccount MapToStudentAccount(RegistrationForm form) {
            return new StudentAccount() {
                SchoolId = form.SchoolId,
                FirstName = form.FirstName,
                LastName = form.FirstName,
                Password = form.Password,
                Email = form.Email
            };
        }
    }
}