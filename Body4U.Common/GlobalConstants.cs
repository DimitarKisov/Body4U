﻿namespace Body4U.Common
{
    public static class GlobalConstants
    {
        #region System
        public const string SystemName = "Body4U";
        public const string AdministratorRoleName = "Administrator";
        public const string UserRoleName = "User";
        public const string TrainerRoleName = "Trainer";
        #endregion

        #region Account
        public const string WrongImageFormat = "Моля изберете снимка с формат .jpg или .png";
        public const string EmailExists = "Вече съществува потребител с такъв email.";
        public const string PleaseConfirmEmail = "Моля потвърдете вашия email.";
        public const string InvalidPasswordResetToken = "Невалиден email или reset token.";
        public const string UnssuccesfulPasswordReset = "Неуспешно подновяване";
        public const string RegistrationUnssuccesful = "Неуспешна регистрация";
        public const string WrongRights = "Нямате права за това действие.";
        public const string TrainerVideoUrl = "Имате зададен грешен линк за видео.";
        public const string MaxTrainerImages = "Не може да имате повече от 6 снимки.";

        public const int MaxTrainerImagesCount = 6;
        public const int MaxTrainerVideosCount = 3;
        #endregion

        #region Article
        public const string ArticleTitleExsists = "Вече съществува статия с такова заглавие.";
        public const string NotReadyToWriteArticle = "Моля попълнете нужната информация за Вас, за да можете, да създадете статия.";
        public const string WrongImageWidthHeight = "Моля въведете снимка с размери минимум 730x548.";
        public const string ArticleMissing = "Не съществува статия с такъв номер.";
        #endregion

        #region Common
        public const string NotFound = "Търсената от Вас страница не може да бъде намерена.";
        public const string Wrong = "Нещо се обърка. Моля опитайте пак!";
        public const string WrongId = "Грешен идентификационен номер.";
        #endregion
    }
}
