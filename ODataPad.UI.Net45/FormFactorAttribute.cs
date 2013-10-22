using Cirrious.CrossCore.IoC;

namespace ODataPad.UI.Net45
{
    public class FormFactorAttribute : MvxConditionalConventionalAttribute
    {
        private readonly bool _satisfied;

        public FormFactorAttribute(bool satistfied)
        {
            _satisfied = satistfied;
        }

        public override bool IsConditionSatisfied
        {
            get { return _satisfied; }
        }
    }

    public class FormFactor
    {
        public const bool Tablet =
#if FORMFACTOR_TABLET
            true;
#else
            false;
#endif

        public const bool Phone =
#if FORMFACTOR_PHONE
            true;
#else
            false;
#endif
    }
}