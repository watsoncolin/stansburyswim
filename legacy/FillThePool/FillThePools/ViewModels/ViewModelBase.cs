using FillThePool.Web.Models;

namespace FillThePool.Web.ViewModels
{
    public class ViewModelBase
    {
        private bool? _canEdit;
        private bool? _canDelete;

        private readonly RoleEvaluator _evaluator = new RoleEvaluator();

        public bool CanEdit
        {
            get
            {
                if (!_canEdit.HasValue)
                {
                    _canEdit = _evaluator.CanEdit;
                }
                return _canEdit.Value;
            }
        }
        public bool CanDelete
        {
            get
            {
                if (!_canDelete.HasValue)
                {
                    _canDelete = _evaluator.CanDelete;
                }
                return _canDelete.Value;
            }
        }

    }
}