using FluentAssertions;
using MapsExt.Compatibility;
using MapsExt.MapObjects;
using MapsExt.Properties;
using Sirenix.Serialization;
using Surity;
using UnityEngine;

namespace MapsExt.Tests
{
	[TestClass]
	public class OldVersionSerializationTests
	{
		private static readonly DeserializationContext deserializationContext = new()
		{
			Config = new SerializationConfig
			{
				DebugContext = new DebugContext
				{
					ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient,
					LoggingPolicy = LoggingPolicy.LogWarningsAndErrors,
					Logger = new SerializationLogger()
				}
			}
		};

		[Test]
		public void Test_LoadV0Map()
		{
			var map = MapLoader.LoadResource("MapsExt.Serialization.Fixtures.0.10.0.map", deserializationContext);
			map.Should().NotBeNull();
			map.MapObjects.Should().NotBeNull();
			map.MapObjects.Length.Should().Be(10);
			map.MapObjects.Should().AllBeAssignableTo(typeof(MapObjectData));

			var ballData = (BallData) map.MapObjects[0];
			ballData.active.Should().Be(true);
			ballData.Position.Should().Be(new PositionProperty(-13.25f, 7.75f));
			ballData.Scale.Should().Be(new ScaleProperty(4, 2));
			ballData.Rotation.Should().Be(new RotationProperty());

			var boxData = (BoxData) map.MapObjects[1];
			boxData.active.Should().Be(true);
			boxData.Position.Should().Be(new PositionProperty(-10.5f, 7.75f));
			boxData.Scale.Should().Be(new ScaleProperty(2, 2));
			boxData.Rotation.Should().Be(new RotationProperty(45));

			var boxBackgroundData = (BoxBackgroundData) map.MapObjects[2];
			boxBackgroundData.active.Should().Be(false);
			boxBackgroundData.Position.Should().Be(new PositionProperty(-8f, 7.75f));
			boxBackgroundData.Scale.Should().Be(new ScaleProperty(2, 2));
			boxBackgroundData.Rotation.Should().Be(new RotationProperty());

			var boxDestructibleData = (BoxDestructibleData) map.MapObjects[3];
			boxDestructibleData.active.Should().Be(true);
			boxDestructibleData.DamageableByEnvironment.Should().Be(new DamageableProperty(true));
			boxDestructibleData.Position.Should().Be(new PositionProperty(-5.5f, 7.75f));
			boxDestructibleData.Scale.Should().Be(new ScaleProperty(2, 2));
			boxDestructibleData.Rotation.Should().Be(new RotationProperty());

			var sawDynamicData = (SawDynamicData) map.MapObjects[4];
			sawDynamicData.active.Should().Be(true);
			sawDynamicData.Position.Should().Be(new PositionProperty(-2.75f, 7.75f));
			sawDynamicData.Scale.Should().Be(new ScaleProperty(2, 2));
			sawDynamicData.Rotation.Should().Be(new RotationProperty());

			var groundData = (GroundData) map.MapObjects[5];
			groundData.active.Should().Be(true);
			groundData.Position.Should().Be(new PositionProperty(-13.25f, 5.25f));
			groundData.Scale.Should().Be(new ScaleProperty(2, 2));
			groundData.Rotation.Should().Be(new RotationProperty());

			var sawData = (SawData) map.MapObjects[6];
			sawData.active.Should().Be(true);
			sawData.Position.Should().Be(new PositionProperty(-8f, 5.25f));
			sawData.Scale.Should().Be(new ScaleProperty(2, 2));
			sawData.Rotation.Should().Be(new RotationProperty());

			var groundCircleData = (GroundCircleData) map.MapObjects[7];
			groundCircleData.active.Should().Be(true);
			groundCircleData.Position.Should().Be(new PositionProperty(-10.5f, 5.25f));
			groundCircleData.Scale.Should().Be(new ScaleProperty(2, 2));
			groundCircleData.Rotation.Should().Be(new RotationProperty());

			var ropeData = (RopeData) map.MapObjects[8];
			ropeData.active.Should().Be(true);
			ropeData.Position.Should().Be(new RopePositionProperty(new Vector2(-6.25f, 6f), new Vector2(-5f, 4.75f)));

			var spawnData = (SpawnData) map.MapObjects[9];
			spawnData.active.Should().Be(true);
			spawnData.Id.Should().Be(new SpawnIDProperty(1, 2));
			spawnData.Position.Should().Be(new PositionProperty(-2.75f, 5.5f));
		}
	}
}
