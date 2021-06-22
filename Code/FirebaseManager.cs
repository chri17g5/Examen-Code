using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    [Header("FireBase")]
    public DependencyStatus dependencyStatus;
    public Firebase.Auth.FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference DBReference;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordVerfiField;
    public TMP_Text warningRegisterText;

    [Header("DataBase")]
    public TMP_InputField fullNameField;
    public TMP_InputField phoneNumField;
    public TMP_InputField dateOfBirthField;

    private void Awake()
    {
        //Check that all of the nescessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initilize firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies " + dependencyStatus);
            }
        });
    }

    public void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentaction instance object
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;  //Minor shout out to lola for the help with this line right here
        DBReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    #region BTN interactions
    #region Clear UI methods
    /// <summary>
    /// Clears LoginUI text fields
    /// </summary>
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    /// <summary>
    /// Clears RegisterUI text fields
    /// </summary>
    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordVerfiField.text = "";
    }
    #endregion

    public void LoginButton()
    {
        //Call the login coroutine passing the emil and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        //Call the register passing the email, password and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.BackToLogin();
        ClearLoginFeilds();
        ClearRegisterFields();
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(fullNameField.text));

        StartCoroutine(UpdateUsernameDatabase(fullNameField.text));
        StartCoroutine(UpdateUserMobileNum(phoneNumField.text));
        StartCoroutine(UpdateUserDOB(dateOfBirthField.text));

        UIManager.instance.CloseUserSettings();
    }
    #endregion

    #region Login and Register Logic
    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until Firebase auth signin function passing the emil and password
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.Log(message: $"Failed to register task with {loginTask.Exception}");
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message; //updates GUI element to display error to user
        }
        else
        {
            //User is now logged in
            //Now get the result
            user = loginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";
            StartCoroutine(LoadUserData());

            fullNameField.text = user.DisplayName;
            UIManager.instance.ToMainFromLogin();
            ClearLoginFeilds();
            ClearRegisterFields();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordVerfiField.text)
        {
            //IF pass words does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are erros handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                //Another grand error message switch case
                string message = "Register Failed";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the results
                user = RegisterTask.Result;
                if (user != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //Id there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed";
                    }
                    else
                    {
                        //Username is now set
                        //Also user is placed back at login menu
                        UIManager.instance.ToLoginFromReg();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }
    #endregion

    #region UpdateMethods
    //---------------------------------------------------
    //  Method(s) below updates the Auth and not DB
    //---------------------------------------------------
    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user progile function passing the profile with the username
        var ProfileTask = user.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth user name has been updated
        }
    }

    //---------------------------------------------------
    //  Method(s) below updates the DB and not Auth
    //---------------------------------------------------
    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Path to username in the DB 
        var DBTask = DBReference.Child("users").Child(user.UserId).Child("username").SetValueAsync(_username);
       
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateUserMobileNum(string _mobileNum)
    {
        //Path to mobileNum in the DB 
        var DBTask = DBReference.Child("users").Child(user.UserId).Child("mobileNum").SetValueAsync(_mobileNum);

        //Wait until the task completes
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }

    private IEnumerator UpdateUserDOB(string _dOB)
    {
        //Path to dateOfBirth in the DB 
        var DBTask = DBReference.Child("users").Child(user.UserId).Child("dateOfBirth").SetValueAsync(_dOB);

        //Wait until the task completes
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }
    #endregion

    #region LoadData
    private IEnumerator LoadUserData() 
    {
        //User ID Path
        var DBTask = DBReference.Child("users").Child(user.UserId).GetValueAsync();

        //Wait until the task completes
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //if no data exists don't post it in the fields
            phoneNumField.text = "";
            dateOfBirthField.text = "";
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result; //Snapshot is baseically mirrored data

            phoneNumField.text = snapshot.Child("mobileNum").Value.ToString();
            dateOfBirthField.text = snapshot.Child("dateOfBirth").Value.ToString();
        }
    }
    #endregion
}
