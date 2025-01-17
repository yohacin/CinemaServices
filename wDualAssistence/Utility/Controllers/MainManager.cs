using Data;

namespace wDualAssistence.Utility.Controllers
{
    public class MainManager
    {
        protected IApplicationDbContext _appCnx;
        protected log4net.ILog _log;

        public MainManager(IApplicationDbContext _appCnx, log4net.ILog _log)
        {
            this._appCnx = _appCnx;
            this._log = _log;
        }

        public MainManager(IApplicationDbContext _appCnx)
        {
            this._appCnx = _appCnx;
            this._log = log4net.LogManager.GetLogger(this.GetType());
        }
    }
}