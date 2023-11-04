using RotationSolver.Basic.Configuration;

namespace KirboRotations.Utility;

public enum KirboPluginConfigFloat : byte
{
	[Default(60f, 10f, 1800f)]
	DeadTimeBoss,
	[Default(10f, 0f, 600f)]
	DeadTimeDying
}
