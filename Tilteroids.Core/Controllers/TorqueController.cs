using SpaceshipArcade.MG.Engine.Utilities;

namespace Tilteroids.Core.Controllers;

public class TorqueController(
	float proportionalGain = 400.0f,
	float derivativeGain = 30.0f,
	float inertia = 1.0f,
	float minTorque = -20.0f,
	float maxTorque = 20.0f)
{
	public float ProportionalGain = proportionalGain;
	public float DerivativeGain = derivativeGain;
	public float Inertia = inertia;
	public float MinTorque = minTorque;
	public float MaxTorque = maxTorque;

	public float ComputeTorque(float currentAngle, float currentAngularVelocity, float targetAngle)
	{
		float angleDiff = PMath.AngleDifference(targetAngle, currentAngle);

		float targetAcceleration = ProportionalGain * angleDiff - DerivativeGain * currentAngularVelocity;

		float torque = Inertia * targetAcceleration;

		return MathHelper.Clamp(torque, MinTorque, MaxTorque);
	}
}