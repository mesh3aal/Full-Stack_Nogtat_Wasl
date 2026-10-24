namespace Application_Layer.Helper
{
    public static class Routing
    {
        public const string ApiPrefix = "api/";
        public const string BaseAdmin = ApiPrefix + "admin/";
        public const string BaseOwner = ApiPrefix + "owner/";
        public const string BaseCustomer = ApiPrefix + "customer/";

        public static class AdminDashboard
        {
            public const string Prefix = BaseAdmin + "dashboard";

            public const string GetLatestRequest = Prefix + "/latest-Request";
            public const string GetRequestStatus = Prefix + "/Request-status";
            public const string GetById = Prefix + "/{id:int}";
        }

        public static class Categories
        {
            public const string Prefix = BaseAdmin + "categories";

            public const string GetAll = Prefix;
            public const string GetCategory = Prefix+"/getCategory";
            public const string GetById = Prefix + "/{id:int}";
            public const string Create = Prefix;
            public const string Update = Prefix ;
            public const string Delete = Prefix + "/{id:int}";
        }


        public static class CustomerCategories
        {
            public const string Prefix = BaseCustomer + "categories";

            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
        }

        public static class CustomerFavourites
        {
            public const string Prefix = BaseCustomer + "favourites";

            public const string GetAll = Prefix;
            public const string Add = Prefix + "/{placeId:int}";
            public const string Remove = Prefix + "/{placeId:int}";
            public const string IsFavourited = Prefix + "/{placeId:int}";
        }

        public static class CustomerPlaces
        {
            public const string placetoredirect = Prefix + "/redirect/{placeName}";
            public const string Prefix = BaseCustomer + "places";
            public const string SearchPlaces = Prefix + "/search/{placeName}";
            public const string SearchPlaceById = Prefix + "/{placeId:int}";
            public const string GetAllPlaces = Prefix + "/getall";
            public const string GetPlacesOfCategory = Prefix + "/resource/{resourcename}";
            public const string FilterPlaces = Prefix + "/Filter";
            public const string GetCategoryPlaces = Prefix + "/category";
            public const string GetCategoryPlacesById = Prefix + "/category/{categoryId:int}";
            public const string GetSubCategoryPlacesById = Prefix + "/subcategory/{subCategoryId:int}";
            public const string PlaceDetails = Prefix + "/{placeId:int}/Detail";
            public const string RatesAndReactions = Prefix + "/{placeId:int}/reactions";
        }

        public static class CustomerSettings
        {
            public const string ChangePassword = BaseCustomer + "changePassword";
            public const string GetProfileInfo = BaseCustomer + "GetProfileInfo";
            public const string ChangeProfile = BaseCustomer + "ChangeProfile";
            public const string Deactivate = BaseCustomer + "deactivate";
            public const string Reactivate = BaseCustomer + "reactivate";
        }


        public static class Requests
        {
            public const string Prefix = BaseAdmin + "requests";

            public const string GetAll = Prefix;
            public const string Details = Prefix + "/{id:int}/details";
            public const string Accept = Prefix + "/{orderId:int}/accept";
            public const string Reject = Prefix + "/reject";
        }

        public static class Stores
        {
            public const string Prefix = BaseAdmin + "stores";

            public const string GetAll = Prefix;
            public const string Statistics = Prefix + "/statistics";
            public const string Suspend = Prefix + "/{id:int}/suspend";
            public const string Activate = Prefix + "/{id:int}/activate";
            public const string Delete = Prefix + "/{id:int}";
            public const string ChangeStatus = Prefix + "/{id:int}/status";
            public const string Edit = Prefix + "/edit";
            public const string GetForEdit = Prefix + "/{id:int}/edit";
        }


        public static class Users
        {
            public const string Prefix = BaseAdmin + "users";

            public const string GetAll = Prefix;
            public const string Suspend = Prefix + "/{id:int}/suspend";
            public const string Activate = Prefix + "/{id:int}/activate";
            public const string ChangeStatus = Prefix + "/{id:int}/status";
            public const string Create = "api/admin/users"; 
            public const string Delete = "api/admin/users"; 
            public const string Update = "api/admin/users"; 
            public const string Find = Prefix+"/find"; 
        }
        public static class OwnerDashboard
        {
            public const string Prefix = BaseOwner + "dashboard";
            public const string Prefixplase = Prefix + "/places";

            public const string GetAlll = Prefixplase;
            public const string Statistics = Prefix + "/statistics";
        }

        public static class OwnerPlaces
        {
            public const string Prefix = BaseOwner + "places";

            public const string GetAll = Prefix;
            public const string Details = Prefix + "/{id:int}/details";
            public const string GetById = Prefix + "/{id:int}";
            public const string Create = Prefix;
            public const string Update = Prefix + "/{id:int}";
            public const string Delete = Prefix + "/{id:int}";
            public const string GetDirectorates = Prefix + "/directorates";
            public const string LatestReviews = Prefix + "/LatestReviews/{id:int}";
        }

        public static class Reviews
        {
            public const string Prefix = ApiPrefix + "reviews";
            public const string GetPlaceReviews = Prefix + "/{placeId:int}";
            public const string GetMyReviews = Prefix + "/myReviews";
            public const string GetReview = Prefix + "/Review/{reviewId:int}";
            public const string AddReview = Prefix + "/{placeId:int}";
            public const string EditReview = Prefix + "/{reviewId}";
            public const string DeleteReview = Prefix + "/{reviewId}";
            public const string isUserHaveReviewOnPlace = Prefix + "/check/{placeId:int}";
            public const string EngageToReview = Prefix + "/{reviewId}/reaction";
            public const string EngagementType = Prefix + "/{reviewId}/reaction-type";
        }

        public static class Subcategories
        {
            public const string Prefix = ApiPrefix + "subcategories";

            public const string GetAllWithCategories = Prefix + "/with-categories";
            public const string GetByCategoryId = Prefix + "/category/{categoryId:int}";
            public const string Create = Prefix;
            public const string Update = Prefix + "/{subCategoryId:int}";
            public const string Delete = Prefix + "/{subCategoryId:int}";
        }

        public static class Authentication
        {
            private const string Prefix = ApiPrefix + "auth";
            
            public const string Register = Prefix + "/register";
            public const string Login = Prefix + "/login";
            public const string RefreshToken = Prefix + "/refresh-token";
            public const string ForgetPassword = Prefix + "/forget-password";
            public const string ResetPassword = Prefix + "/reset-password";
            public const string IsEmailVerified = Prefix + "/is-email-verified";
            public const string EmailConfirm = Prefix + "/email-confirm";
            public const string EmailChangeConfirm = Prefix + "/ChangeEmailConfirm";
            public const string ResendEmailConfirmation = Prefix + "/resend-email-confirmation";
            public const string Logout = Prefix + "/logout";
        }
        public static class Notification
        {
            private const string Prefix = ApiPrefix + "notifications";
            public const string Notifications = Prefix + "/AllNotifications";
            
        }
    }
}
