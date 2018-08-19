// ts2fable 0.6.1
module rec PixiReact
open System
open Fable.Core
open Fable.Import.JS

module PIXI = PixiJs

//let [<Import("*","Pixi.js")>] ``react-pixi-fiber``: React_pixi_fiber.IExports = jsNative

//module React_pixi_fiber =

//    type [<AllowNullLiteral>] IExports =
//        abstract BitmapText: BitmapTextStatic
//        abstract Container: ContainerStatic
//        abstract Graphics: GraphicsStatic
//        abstract ParticleContainer: ParticleContainerStatic
//        abstract Sprite: SpriteStatic
//        abstract Text: TextStatic
//        abstract TilingSprite: TilingSpriteStatic
//        abstract Stage: StageStatic
//        /// Custom React Reconciler render method.
//        abstract render: pixiElement: U2<PIXI.DisplayObject, ResizeArray<PIXI.DisplayObject>> * stage: PIXI.Container * ?callback: Function -> unit
//        /// Create a custom component.
//        abstract CustomPIXIComponent: behavior: Behavior<'T, 'U> * ``type``: string -> React.ReactType<'T>

//    type Omit<'T, 'K> =
//        obj

//    /// The shape of an object that has an optional `children` property of any type.
//    type [<AllowNullLiteral>] ObjectWithChildren =
//        abstract children: obj option with get, set

//    type Childless<'T> =
//        Omit<'T, string>

//    /// The shape of a component that has an optional `children` property.
//    type [<AllowNullLiteral>] ChildrenProperties =
//        abstract children: React.ReactNode option with get, set

//    type ChildlessComponent<'T> =
//        obj

//    type [<AllowNullLiteral>] Component<'T> =
//        interface end

//    /// `BitmapText` component properties.
//    type [<AllowNullLiteral>] BitmapTextProperties =
//        inherit ChildlessComponent<PIXI.Extras.BitmapText>
//        abstract text: string with get, set

//    /// A component wrapper for `PIXI.extras.BitmapText`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.extras.BitmapText.html
//    type [<AllowNullLiteral>] BitmapText =
//        inherit React.Component<BitmapTextProperties>

//    /// A component wrapper for `PIXI.extras.BitmapText`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.extras.BitmapText.html
//    type [<AllowNullLiteral>] BitmapTextStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> BitmapText

//    /// `Container` component properties.
//    type [<AllowNullLiteral>] ContainerProperties =
//        inherit ChildlessComponent<PIXI.Container>

//    /// A component wrapper for `PIXI.extras.BitmapText`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Container.html
//    type [<AllowNullLiteral>] Container =
//        inherit React.Component<ContainerProperties>

//    /// A component wrapper for `PIXI.extras.BitmapText`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Container.html
//    type [<AllowNullLiteral>] ContainerStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> Container

//    /// `Graphics` component properties.
//    type [<AllowNullLiteral>] GraphicsProperties =
//        inherit Component<PIXI.Graphics>

//    /// A component wrapper for `PIXI.Graphics`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Graphics.html
//    type [<AllowNullLiteral>] Graphics =
//        inherit React.Component<GraphicsProperties>

//    /// A component wrapper for `PIXI.Graphics`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Graphics.html
//    type [<AllowNullLiteral>] GraphicsStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> Graphics

//    /// `ParticleContainer` component properties.
//    type [<AllowNullLiteral>] ParticleContainerProperties =
//        inherit ChildlessComponent<PIXI.Particles.ParticleContainer>

//    /// A component wrapper for `PIXI.particles.ParticleContainer`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.particles.ParticleContainer.html
//    type [<AllowNullLiteral>] ParticleContainer =
//        inherit React.Component<TilingSpriteProperties>

//    /// A component wrapper for `PIXI.particles.ParticleContainer`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.particles.ParticleContainer.html
//    type [<AllowNullLiteral>] ParticleContainerStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> ParticleContainer

//    /// `Sprite` component properties.
//    type [<AllowNullLiteral>] SpriteProperties =
//        inherit ChildlessComponent<PIXI.Sprite>

//    /// A component wrapper for `PIXI.Sprite`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Sprite.html
//    type [<AllowNullLiteral>] Sprite =
//        inherit React.Component<SpriteProperties>

//    /// A component wrapper for `PIXI.Sprite`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Sprite.html
//    type [<AllowNullLiteral>] SpriteStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> Sprite

//    /// `Text` component properties
//    type [<AllowNullLiteral>] TextProperties =
//        inherit ChildlessComponent<PIXI.Text>

//    /// A component wrapper for `PIXI.Text`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Text.html
//    type [<AllowNullLiteral>] Text =
//        inherit React.Component<TextProperties>

//    /// A component wrapper for `PIXI.Text`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Text.html
//    type [<AllowNullLiteral>] TextStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> Text

//    /// `TilingSprite` component properties.
//    type [<AllowNullLiteral>] TilingSpriteProperties =
//        inherit ChildlessComponent<PIXI.Extras.TilingSprite>
//        abstract texture: PIXI.Texture with get, set

//    /// A component wrapper for `PIXI.extras.TilingSprite`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.extras.TilingSprite.html
//    type [<AllowNullLiteral>] TilingSprite =
//        inherit React.Component<TilingSpriteProperties>

//    /// A component wrapper for `PIXI.extras.TilingSprite`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.extras.TilingSprite.html
//    type [<AllowNullLiteral>] TilingSpriteStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> TilingSprite

//    /// `Stage` component properties."
//    type [<AllowNullLiteral>] StageProperties =
//        inherit Component<PIXI.Container>
//        abstract options: PIXI.ApplicationOptions option with get, set

//    /// A component wrapper for `PIXI.Application`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Application.html
//    type [<AllowNullLiteral>] Stage =
//        inherit React.Component<StageProperties>

//    /// A component wrapper for `PIXI.Application`.
//    ///
//    /// see: http://pixijs.download/dev/docs/PIXI.Application.html
//    type [<AllowNullLiteral>] StageStatic =
//        [<Emit "new $0($1...)">] abstract Create: unit -> Stage

//    /// Custom component properties.
//    type [<AllowNullLiteral>] Behavior<'T, 'U> =
//        /// Use this to create an instance of [PIXI.DisplayObject].
//        abstract customDisplayObject: ('T -> 'U) with get, set
//        /// Use this to apply newProps to your Component in a custom way.
//        abstract customApplyProps: ('U -> 'T -> 'T -> obj option) option with get, set
//        /// Use this to do something after displayObject is attached, which happens after componentDidMount lifecycle method.
//        abstract customDidAttach: ('U -> obj option) option with get, set
//        /// Use this to do something (usually cleanup) before detaching, which happens before componentWillUnmount lifecycle method.
//        abstract customWillDetach: ('U -> obj option) option with get, set