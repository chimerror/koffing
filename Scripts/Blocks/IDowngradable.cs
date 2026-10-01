using System.Collections.Generic;

namespace Koffing.Blocks;

public interface IDowngradable<T>
{
	IEnumerable<MadeBlockContext> Downgrade();
}