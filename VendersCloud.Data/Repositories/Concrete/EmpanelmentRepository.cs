﻿namespace VendersCloud.Data.Repositories.Concrete
{
    public class EmpanelmentRepository : StaticBaseRepository<Empanelment>, IEmpanelmentRepository
    {
        public EmpanelmentRepository(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
