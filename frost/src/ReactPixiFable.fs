module Fable.Import.ReactPixiFable
open Fable.Core
open Fable.Helpers.React
open Fable.Import.React

  [<Pojo>]
  type ApplicationOptions = { backgroundColor: int }

  [<Pojo>]
  /// `Stage` component properties."
  type StageProperties =
    {
      width: int
      height: int
      options: ApplicationOptions
    }

  type Point = {
    x: int
    y: int
  }

  type TextStyle = {
    fill: string
  }

  [<Import("Texture", "pixi.js")>]
  [<AbstractClass>]
  type Texture =
    abstract member valid: bool with get, set
    static member fromImage(imageUrl: string, ?crossOrigin: bool) : Texture = jsNative

  [<Pojo>]
  type SpriteProperties = {
    position: Point
    height: int
    width: int
    alpha: float
    texture: Texture
  }

  [<Pojo>]
  type TextProperties = {
    position: Point
    height: int
    width: int
    alpha: float
    text: string
    style: TextStyle
  }

  let stage (props: StageProperties) children : ReactElement = ofImport "Stage" "react-pixi-fiber" props children
  let text (props: TextProperties) children = ofImport "Text" "react-pixi-fiber" props children
  let sprite (props: SpriteProperties) children = ofImport "Sprite" "react-pixi-fiber" props children
