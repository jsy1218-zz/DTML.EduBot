namespace DTML.EduBot.Qna
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    interface IQnaService
    {
        Task<IQnaResult> QueryAsync(string question, CancellationToken token);

    }
}
