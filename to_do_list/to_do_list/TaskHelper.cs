using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace To_Do_List_2
{
    public static class TaskHelper
    {
        
        public static ConfiguredTaskAwaitable<T> StartAsTask<T>(
    this IAsyncOperation<T> self, bool continueOnContext)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            return self.AsTask().ConfigureAwait(continueOnContext);
        }
    }
}
