// Copyright (C) 2006-2013 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.Renderer;
using Engine.MathEx;
using Engine.SoundSystem;
using Engine.UISystem;
using Engine.EntitySystem;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using Engine.FileSystem;
using Engine.Utils;
using GameCommon;
using GameEntities;

namespace AOT
{
	public class AOTGameWindow : GameWindow
	{
		enum CameraType
		{
			Game,
			Free,

			Count
		}
		static CameraType cameraType;

		[Config( "Map", "drawPathMotionMap" )]
		public static bool mapDrawPathMotionMap;

		Range cameraDistanceRange = new Range( 10, 300 );
		Range cameraAngleRange = new Range( .001f, MathFunctions.PI / 2 - .001f );
		float cameraDistance = 23;
		SphereDir cameraDirection = new SphereDir( 1.5f, .85f );
		Vec2 cameraPosition;

		
		float timeForUpdateGameStatus;

		ScrollBar cameraDistanceScrollBar;
		ScrollBar cameraHeightScrollBar;
		bool disableUpdatingCameraScrollBars;

		//

		protected override void OnAttach()
		{
			base.OnAttach();

			EngineApp.Instance.KeysAndMouseButtonUpAll();

			//set camera position
			foreach( Entity entity in Map.Instance.Children )
			{
				SpawnPoint spawnPoint = entity as SpawnPoint;
				if( spawnPoint == null )
					continue;
				cameraPosition = spawnPoint.Position.ToVec2();
				break;
			}

			

			ResetTime();

			//render scene for loading resources
			EngineApp.Instance.RenderScene();

			EngineApp.Instance.MousePosition = new Vec2( .5f, .5f );

		}

		public override void OnBeforeWorldSave()
		{
			base.OnBeforeWorldSave();

			//World serialized data
			World.Instance.ClearAllCustomSerializationValues();
		}

		protected override void OnDetach()
		{
			//minimap
			

			base.OnDetach();
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			//If atop openly any window to not process
			if( IsPaused() )
				return base.OnKeyDown( e );

			//change camera type
			if( e.Key == EKeys.F7 )
			{
				cameraType = (CameraType)( (int)GetRealCameraType() + 1 );
				if( cameraType == CameraType.Count )
					cameraType = (CameraType)0;

				FreeCameraEnabled = cameraType == CameraType.Free;

				GameEngineApp.Instance.AddScreenMessage( "Camera type: " + cameraType.ToString() );

				return true;
			}

            //if (e.Key == EKeys.Escape)
            //{
            //    Controls.Add(new MenuWindow());
            //}

			return base.OnKeyDown( e );
		}

		protected override bool OnKeyUp( KeyEvent e )
		{
			//If atop openly any window to not process
			if( IsPaused() )
				return base.OnKeyUp( e );

			return base.OnKeyUp( e );
		}


		protected override bool OnMouseDown( EMouseButtons button )
		{
			//If atop openly any window to not process
			if( IsPaused() )
				return base.OnMouseDown( button );


			return base.OnMouseDown( button );
		}

		protected override bool OnMouseUp( EMouseButtons button )
		{
			//If atop openly any window to not process
			if( IsPaused() )
				return base.OnMouseUp( button );

			return base.OnMouseUp( button );
		}

		protected override bool OnMouseDoubleClick( EMouseButtons button )
		{
			//If atop openly any window to not process
			if( IsPaused() )
				return base.OnMouseDoubleClick( button );

			return base.OnMouseDoubleClick( button );
		}

		protected override void OnMouseMove()
		{
			base.OnMouseMove();

			//If atop openly any window to not process
			if( IsPaused() )
				return;

		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			//If atop openly any window to not process
            if (IsPaused())
                return;

			if( !FreeCameraMouseRotating )
				EngineApp.Instance.MouseRelativeMode = false;

			bool activeConsole = EngineConsole.Instance != null && EngineConsole.Instance.Active;

			if( GetRealCameraType() == CameraType.Game && !activeConsole )
			{
				if( EngineApp.Instance.IsKeyPressed( EKeys.PageUp ) )
				{
					cameraDistance -= delta * ( cameraDistanceRange[ 1 ] - cameraDistanceRange[ 0 ] ) / 10.0f;
					if( cameraDistance < cameraDistanceRange[ 0 ] )
						cameraDistance = cameraDistanceRange[ 0 ];
				}

				if( EngineApp.Instance.IsKeyPressed( EKeys.PageDown ) )
				{
					cameraDistance += delta * ( cameraDistanceRange[ 1 ] - cameraDistanceRange[ 0 ] ) / 10.0f;
					if( cameraDistance > cameraDistanceRange[ 1 ] )
						cameraDistance = cameraDistanceRange[ 1 ];
				}

				//rtsCameraDirection

				if( EngineApp.Instance.IsKeyPressed( EKeys.Home ) )
				{
					cameraDirection.Vertical += delta * ( cameraAngleRange[ 1 ] - cameraAngleRange[ 0 ] ) / 2;
					if( cameraDirection.Vertical >= cameraAngleRange[ 1 ] )
						cameraDirection.Vertical = cameraAngleRange[ 1 ];
				}

				if( EngineApp.Instance.IsKeyPressed( EKeys.End ) )
				{
					cameraDirection.Vertical -= delta * ( cameraAngleRange[ 1 ] - cameraAngleRange[ 0 ] ) / 2;
					if( cameraDirection.Vertical < cameraAngleRange[ 0 ] )
						cameraDirection.Vertical = cameraAngleRange[ 0 ];
				}

				if( EngineApp.Instance.IsKeyPressed( EKeys.Q ) )
				{
					cameraDirection.Horizontal += delta * 2;
					if( cameraDirection.Horizontal >= MathFunctions.PI * 2 )
						cameraDirection.Horizontal -= MathFunctions.PI * 2;
				}

				if( EngineApp.Instance.IsKeyPressed( EKeys.E ) )
				{
					cameraDirection.Horizontal -= delta * 2;
					if( cameraDirection.Horizontal < 0 )
						cameraDirection.Horizontal += MathFunctions.PI * 2;
				}


				//change cameraPosition
				if(Time > 2 )
				{
					Vec2 vector = Vec2.Zero;

					if( EngineApp.Instance.IsKeyPressed( EKeys.Left ) ||
						EngineApp.Instance.IsKeyPressed( EKeys.A ) || MousePosition.X < .005f )
					{
						vector.X--;
					}
					if( EngineApp.Instance.IsKeyPressed( EKeys.Right ) ||
						EngineApp.Instance.IsKeyPressed( EKeys.D ) || MousePosition.X > 1.0f - .005f )
					{
						vector.X++;
					}
					if( EngineApp.Instance.IsKeyPressed( EKeys.Up ) ||
						EngineApp.Instance.IsKeyPressed( EKeys.W ) || MousePosition.Y < .005f )
					{
						vector.Y++;
					}
					if( EngineApp.Instance.IsKeyPressed( EKeys.Down ) ||
						EngineApp.Instance.IsKeyPressed( EKeys.S ) || MousePosition.Y > 1.0f - .005f )
					{
						vector.Y--;
					}

					if( vector != Vec2.Zero )
					{
						//rotate vector
						float angle = MathFunctions.ATan( -vector.Y, vector.X ) +
							cameraDirection.Horizontal;
						vector = new Vec2( MathFunctions.Sin( angle ), MathFunctions.Cos( angle ) );

						cameraPosition += vector * delta * 50;
					}
				}

			}


		}

		protected override void OnRender()
		{
			base.OnRender();

			if( GridPathFindSystem.Instances.Count != 0 )
				GridPathFindSystem.Instances[ 0 ].DebugDraw = mapDrawPathMotionMap;
		}



		

		

		

		

		

		

		

			

		protected override void OnRenderUI( GuiRenderer renderer )
		{
			base.OnRenderUI( renderer );
		}

		CameraType GetRealCameraType()
		{
			return cameraType;
		}


		

		protected override void OnGetCameraTransform( out Vec3 position, out Vec3 forward,
			out Vec3 up, ref Degree cameraFov )
		{
			Vec3 offset;
			{
				Quat rot = new Angles( 0, 0, MathFunctions.RadToDeg(
					cameraDirection.Horizontal ) ).ToQuat();
				rot *= new Angles( 0, MathFunctions.RadToDeg( cameraDirection.Vertical ), 0 ).ToQuat();
				offset = rot * new Vec3( 1, 0, 0 );
				offset *= cameraDistance;
			}
			Vec3 lookAt = new Vec3( cameraPosition.X, cameraPosition.Y, 0 );

			position = lookAt + offset;
			forward = -offset;
			up = new Vec3( 0, 0, 1 );
		}

	}
}
