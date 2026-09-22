using System.Collections.Generic;

public interface IDowngradable<T>
{
	IEnumerable<MadeBlockContext> Downgrade();
}