namespace Domain_Layer.Exceptions;

public class CustomErrorDescriper : IdentityErrorDescriber
{
public override IdentityError DefaultError() 
{ 
    return new IdentityError { Code = nameof(DefaultError), Description = "حدث خطأ غير معروف." }; 
}

public override IdentityError ConcurrencyFailure() 
{ 
    return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "فشل في التعارض المتزامن (Concurrency)، لقد تم تعديل الكائن مسبقاً." }; 
}

public override IdentityError PasswordMismatch() 
{ 
    return new IdentityError { Code = nameof(PasswordMismatch), Description = "كلمة المرور غير صحيحة." }; 
}

public override IdentityError InvalidToken() 
{ 
    return new IdentityError { Code = nameof(InvalidToken), Description = "الرمز (Token) غير صالح." }; 
}

public override IdentityError LoginAlreadyAssociated() 
{ 
    return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "يوجد مستخدم مرتبط ببيانات الدخول هذه بالفعل." }; 
}

public override IdentityError InvalidUserName(string userName) 
{ 
    return new IdentityError { Code = nameof(InvalidUserName), Description = $"اسم المستخدم '{userName}' غير صالح، يجب أن يحتوي على حروف أو أرقام فقط." }; 
}

public override IdentityError InvalidEmail(string email) 
{ 
    return new IdentityError { Code = nameof(InvalidEmail), Description = $"البريد الإلكتروني '{email}' غير صالح." }; 
}

public override IdentityError DuplicateUserName(string userName) 
{ 
    return new IdentityError { Code = nameof(DuplicateUserName), Description = $"اسم المستخدم '{userName}' مستخدم بالفعل." }; 
}

public override IdentityError DuplicateEmail(string email) 
{ 
    return new IdentityError { Code = nameof(DuplicateEmail), Description = $"البريد الإلكتروني '{email}' مستخدم بالفعل." }; 
}

public override IdentityError InvalidRoleName(string role) 
{ 
    return new IdentityError { Code = nameof(InvalidRoleName), Description = $"الدور/الصلاحية '{role}' غير صالحة." }; 
}

public override IdentityError DuplicateRoleName(string role) 
{ 
    return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"الدور/الصلاحية '{role}' مستخدمة بالفعل." }; 
}

public override IdentityError UserAlreadyHasPassword() 
{ 
    return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "المستخدم لديه كلمة مرور محددة بالفعل." }; 
}

public override IdentityError UserLockoutNotEnabled() 
{ 
    return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "خاصية قفل الحساب (Lockout) غير مفعلة لهذا المستخدم." }; 
}

public override IdentityError UserAlreadyInRole(string role) 
{ 
    return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"المستخدم يمتلك بالفعل صلاحية '{role}'." }; 
}

public override IdentityError UserNotInRole(string role) 
{ 
    return new IdentityError { Code = nameof(UserNotInRole), Description = $"المستخدم لا يمتلك صلاحية '{role}'." }; 
}

public override IdentityError PasswordTooShort(int length) 
{ 
    return new IdentityError { Code = nameof(PasswordTooShort), Description = $"يجب أن تتكون كلمة المرور من {length} رموز على الأقل." }; 
}

public override IdentityError PasswordRequiresNonAlphanumeric() 
{ 
    return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "يجب أن تحتوي كلمة المرور على رمز واحد غير أبجدي رقمي (مثل @، #، $)." }; 
}

public override IdentityError PasswordRequiresDigit() 
{ 
    return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل ('0'-'9')." }; 
}

public override IdentityError PasswordRequiresLower() 
{ 
    return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل ('a'-'z')." }; 
}

public override IdentityError PasswordRequiresUpper() 
{ 
    return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل ('A'-'Z')." }; 
}
}