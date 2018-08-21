// ts2fable 0.6.1
module rec PixiReact
open System
open Fable.Core
open Fable.Import.JS

module PIXI = PixiJs.PIXI

let [<Import("*","react-pixi-fiber")>] rpf: React_pixi_fiber.IExports = jsNative

module React_pixi_fiber =
    open Fable.Import

    type IExports =
        abstract BitmapText: BitmapTextStatic
        abstract Container: ContainerStatic
        abstract Graphics: GraphicsStatic
        abstract ParticleContainer: ParticleContainerStatic
        abstract Sprite: SpriteStatic
        abstract Text: TextStatic
        abstract TilingSprite: TilingSpriteStatic
        abstract Stage: StageStatic
        /// Custom React Reconciler render method.
        abstract render: pixiElement: U2<PIXI.DisplayObject, ResizeArray<PIXI.DisplayObject>> * stage: PIXI.Container * ?callback: Function -> unit
        /// Create a custom component.
        abstract CustomPIXIComponent: behavior: Behavior<'T, 'U> * ``type``: string -> React.ReactType

    /// The shape of an object that has an optional `children` property of any type.
    type ObjectWithChildren =
        abstract children: obj option with get, set

    type Childless<'T> = interface end

    /// The shape of a component that has an optional `children` property.
    type ChildrenProperties =
        abstract children: React.ReactNode option with get, set

    [<AbstractClass>]
    type ChildlessComponent<'T>(props:'T) =
        inherit React.Component<'T, unit>(props)

    [<AbstractClass>]
    type Component<'T>(props:'T) =
        inherit React.Component<'T, unit>(props)

    /// `BitmapText` component properties.
    type BitmapTextProperties =
        inherit PIXI.Extras.BitmapText
        abstract text: string with get, set

    /// A component wrapper for `PIXI.extras.BitmapText`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.extras.BitmapText.html
    type BitmapText =
        React.Component<BitmapTextProperties, unit>

    /// A component wrapper for `PIXI.extras.BitmapText`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.extras.BitmapText.html
    type BitmapTextStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> BitmapText

    /// `Container` component properties.
    type ContainerProperties =
        obj

    /// A component wrapper for `PIXI.extras.BitmapText`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Container.html
    type Container =
        Component<ContainerProperties>

    /// A component wrapper for `PIXI.extras.BitmapText`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Container.html
    type ContainerStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> Container

    /// `Graphics` component properties.
    type GraphicsProperties =
        Component<PIXI.Graphics>

    /// A component wrapper for `PIXI.Graphics`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Graphics.html
    type Graphics =
        Component<GraphicsProperties>

    /// A component wrapper for `PIXI.Graphics`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Graphics.html
    type GraphicsStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> Graphics

    /// `ParticleContainer` component properties.
    type ParticleContainerProperties =
        PIXI.Particles.ParticleContainer

    /// A component wrapper for `PIXI.particles.ParticleContainer`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.particles.ParticleContainer.html
    type ParticleContainer =
        Component<TilingSpriteProperties>

    /// A component wrapper for `PIXI.particles.ParticleContainer`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.particles.ParticleContainer.html
    type ParticleContainerStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> ParticleContainer

    /// `Sprite` component properties.
    type SpriteProperties =
        PIXI.Sprite

    /// A component wrapper for `PIXI.Sprite`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Sprite.html
    type Sprite =
        Component<SpriteProperties>

    /// A component wrapper for `PIXI.Sprite`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Sprite.html
    type SpriteStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> Sprite

    /// `Text` component properties
    type TextProperties =
        PIXI.Text

    /// A component wrapper for `PIXI.Text`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Text.html
    type Text =
        Component<TextProperties>

    /// A component wrapper for `PIXI.Text`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Text.html
    type TextStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> Text

    /// `TilingSprite` component properties.
    type TilingSpriteProperties =
        inherit PIXI.Extras.TilingSprite
        abstract texture: PIXI.Texture with get, set

    /// A component wrapper for `PIXI.extras.TilingSprite`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.extras.TilingSprite.html
    type TilingSprite =
        Component<TilingSpriteProperties>

    /// A component wrapper for `PIXI.extras.TilingSprite`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.extras.TilingSprite.html
    type TilingSpriteStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> TilingSprite

    /// `Stage` component properties."
    type StageProperties =
        inherit PIXI.Container
        abstract options: PIXI.ApplicationOptions option with get, set

    /// A component wrapper for `PIXI.Application`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Application.html
    [<AbstractClass>]
    type Stage(props) =
        inherit Component<StageProperties>(props)
        [<Emit "new $0($1...)">] abstract Create: StageProperties -> Stage

    /// A component wrapper for `PIXI.Application`.
    ///
    /// see: http://pixijs.download/dev/docs/PIXI.Application.html
    type StageStatic =
        [<Emit "new $0($1...)">] abstract Create: StageProperties -> Stage

    /// Custom component properties.
    type Behavior<'T, 'U> =
        /// Use this to create an instance of [PIXI.DisplayObject].
        abstract customDisplayObject: ('T -> 'U) with get, set
        /// Use this to apply newProps to your Component in a custom way.
        abstract customApplyProps: ('U -> 'T -> 'T -> obj option) option with get, set
        /// Use this to do something after displayObject is attached, which happens after componentDidMount lifecycle method.
        abstract customDidAttach: ('U -> obj option) option with get, set
        /// Use this to do something (usually cleanup) before detaching, which happens before componentWillUnmount lifecycle method.
        abstract customWillDetach: ('U -> obj option) option with get, set