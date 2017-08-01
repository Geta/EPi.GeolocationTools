using EPiServer.Globalization;

namespace Geta.Epi.GeolocationRedirect
{
    public class RedirectViewModel
    {
        public RedirectViewModel(RedirectBlock block)
        {
            Block = block;
        }

        public RedirectBlock Block { get; set; }
        public string CurrentLanguage { get; set; } = ContentLanguage.PreferredCulture.TwoLetterISOLanguageName;
        public string CurrentLanguageBasedOnIp { get; set; }
    }
}