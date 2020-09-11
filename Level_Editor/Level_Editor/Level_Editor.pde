public MapHandler mapHandler;
public Camera camera;
public MCHandler mCHandler;

void setup(){
  //Environment Setup
  fullScreen();
  frameRate(60);
  
  //Class Setups//
  mapHandler = new MapHandler();
  camera = new Camera();
  mCHandler = new MCHandler();
}

void draw(){
  background(255);
  //Main//
  mCHandler.Update();
  camera.Update();
  mapHandler.Update();
  EntityHandler.Update();
  
  mCHandler.PostUpdate();
  //Draw//
  mapHandler.Draw();
  EntityHandler.Draw();
}

public class Vector2{
  public float X;
  public float Y;
  Vector2(float x, float y){
    X = x;
    Y = y;
  }
}
