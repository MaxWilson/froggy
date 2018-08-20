// ts2fable 0.6.1
module rec PixiJs
open System
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser


module PIXI =
    let [<Import("accessibility","pixi.js")>] accessibility: Accessibility.IExports = jsNative
    let [<Import("CONST","pixi.js")>] Const: CONST.IExports = jsNative
    let [<Import("CanvasTinter","pixi.js")>] canvasTinter: CanvasTinter.IExports = jsNative
    let [<Import("extract","pixi.js")>] extract: Extract.IExports = jsNative
    let [<Import("extras","pixi.js")>] extras: Extras.IExports = jsNative
    let [<Import("filters","pixi.js")>] filters: Filters.IExports = jsNative
    let [<Import("glCore","pixi.js")>] glCore: GlCore.IExports = jsNative
    let [<Import("GroupD8","pixi.js")>] groupD8: GroupD8.IExports = jsNative
    let [<Import("interaction","pixi.js")>] interaction: Interaction.IExports = jsNative
    let [<Import("loaders","pixi.js")>] loaders: Loaders.IExports = jsNative
    let [<Import("mesh","pixi.js")>] mesh: Mesh.IExports = jsNative
    let [<Import("particles","pixi.js")>] particles: Particles.IExports = jsNative
    let [<Import("prepare","pixi.js")>] prepare: Prepare.IExports = jsNative
    let [<Import("settings","pixi.js")>] settings: Settings.IExports = jsNative
    let [<Import("ticker","pixi.js")>] ticker: Ticker.IExports = jsNative
    let [<Import("utils","pixi.js")>] utils: Utils.IExports = jsNative

    type [<AllowNullLiteral>] IExports =
        abstract VERSION: obj
        abstract PI_2: obj
        abstract RAD_TO_DEG: obj
        abstract DEG_TO_RAD: obj
        abstract RENDERER_TYPE: obj
        abstract BLEND_MODES: obj
        abstract DRAW_MODES: obj
        abstract SCALE_MODES: obj
        abstract WRAP_MODES: obj
        abstract TRANSFORM_MODE: obj
        abstract PRECISION: obj
        abstract GC_MODES: obj
        abstract SHAPES: obj
        abstract TEXT_GRADIENT: obj
        abstract UPDATE_PRIORITY: obj
        abstract autoDetectRenderer: width: float * height: float * ?options: PIXI.RendererOptions * ?forceCanvas: bool -> U2<PIXI.WebGLRenderer, PIXI.CanvasRenderer>
        abstract autoDetectRenderer: ?options: PIXI.RendererOptions -> U2<PIXI.WebGLRenderer, PIXI.CanvasRenderer>
        abstract loader: PIXI.Loaders.Loader
        abstract Application: ApplicationStatic
        abstract Bounds: BoundsStatic
        abstract Container: ContainerStatic
        abstract DisplayObject: DisplayObjectStatic
        abstract Transform: Transform
        abstract GraphicsData: GraphicsDataStatic
        abstract Graphics: GraphicsStatic
        abstract CanvasGraphicsRenderer: CanvasGraphicsRendererStatic
        abstract GraphicsRenderer: GraphicsRendererStatic
        abstract WebGLGraphicsData: WebGLGraphicsDataStatic
        abstract PrimitiveShader: PrimitiveShaderStatic
        abstract Matrix: MatrixStatic
        abstract PointLike: PointLikeStatic
        abstract ObservablePoint: ObservablePointStatic
        abstract Point: PointStatic
        abstract Circle: CircleStatic
        abstract Ellipse: EllipseStatic
        abstract Polygon: PolygonStatic
        abstract Rectangle: RectangleStatic
        abstract RoundedRectangle: RoundedRectangleStatic
        abstract SystemRenderer: SystemRendererStatic
        abstract CanvasRenderer: CanvasRendererStatic
        abstract CanvasMaskManager: CanvasMaskManagerStatic
        abstract CanvasRenderTarget: CanvasRenderTargetStatic
        abstract WebGLRenderer: WebGLRendererStatic
        abstract WebGLState: WebGLStateStatic
        abstract TextureManager: TextureManagerStatic
        abstract TextureGarbageCollector: TextureGarbageCollectorStatic
        abstract ObjectRenderer: ObjectRendererStatic
        abstract Quad: QuadStatic
        abstract RenderTarget: RenderTargetStatic
        abstract BlendModeManager: BlendModeManagerStatic
        abstract FilterManager: FilterManagerStatic
        abstract StencilMaskStack: StencilMaskStackStatic
        abstract MaskManager: MaskManagerStatic
        abstract StencilManager: StencilManagerStatic
        abstract WebGLManager: WebGLManagerStatic
        abstract Filter: FilterStatic
        abstract SpriteMaskFilter: SpriteMaskFilterStatic
        abstract Sprite: SpriteStatic
        abstract BatchBuffer: BatchBufferStatic
        abstract SpriteRenderer: SpriteRendererStatic
        abstract CanvasSpriteRenderer: CanvasSpriteRendererStatic
        abstract TextStyle: TextStyleStatic
        abstract TextMetrics: TextMetricsStatic
        abstract Text: TextStatic
        abstract BaseRenderTexture: BaseRenderTextureStatic
        abstract BaseTexture: BaseTextureStatic
        abstract RenderTexture: RenderTextureStatic
        abstract Texture: TextureStatic
        abstract TextureMatrix: TextureMatrixStatic
        abstract TextureUvs: TextureUvsStatic
        abstract Spritesheet: SpritesheetStatic
        abstract VideoBaseTexture: VideoBaseTextureStatic
        abstract Shader: ShaderStatic
        abstract MiniSignalBinding: MiniSignalBindingStatic
        abstract MiniSignal: MiniSignalStatic

    module Settings =

        type [<AllowNullLiteral>] IExports =
            abstract TARGET_FPMS: float
            abstract MIPMAP_TEXTURES: bool
            abstract RESOLUTION: float
            abstract FILTER_RESOLUTION: float
            abstract SPRITE_MAX_TEXTURES: float
            abstract SPRITE_BATCH_SIZE: float
            abstract RETINA_PREFIX: RegExp
            abstract RENDER_OPTIONS: obj
            abstract TRANSFORM_MODE: float
            abstract GC_MODE: float
            abstract GC_MAX_IDLE: float
            abstract GC_MAX_CHECK_COUNT: float
            abstract WRAP_MODE: float
            abstract SCALE_MODE: float
            abstract PRECISION_VERTEX: string
            abstract PRECISION_FRAGMENT: string
            abstract PRECISION: string
            abstract UPLOADS_PER_FRAME: float
            abstract CAN_UPLOAD_SAME_BUFFER: bool
            abstract MESH_CANVAS_PADDING: float

        type PRECISION =
            float

    module Accessibility =

        type [<AllowNullLiteral>] IExports =
            abstract AccessibilityManager: AccessibilityManagerStatic

        type [<AllowNullLiteral>] AccessibilityManager =
            abstract activate: unit -> unit
            abstract deactivate: unit -> unit
            abstract div: HTMLElement with get, set
            abstract pool: ResizeArray<HTMLElement> with get, set
            abstract renderId: float with get, set
            abstract debug: bool with get, set
            abstract renderer: SystemRenderer with get, set
            abstract children: ResizeArray<AccessibleTarget> with get, set
            abstract isActive: bool with get, set
            abstract updateAccessibleObjects: displayObject: DisplayObject -> unit
            abstract update: unit -> unit
            abstract capHitArea: hitArea: HitArea -> unit
            abstract addChild: displayObject: DisplayObject -> unit
            abstract _onClick: e: Interaction.InteractionEvent -> unit
            abstract _onFocus: e: Interaction.InteractionEvent -> unit
            abstract _onFocusOut: e: Interaction.InteractionEvent -> unit
            abstract _onKeyDown: e: Interaction.InteractionEvent -> unit
            abstract _onMouseMove: e: MouseEvent -> unit
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] AccessibilityManagerStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: U2<CanvasRenderer, WebGLRenderer> -> AccessibilityManager

        type [<AllowNullLiteral>] AccessibleTarget =
            abstract accessible: bool with get, set
            abstract accessibleTitle: string option with get, set
            abstract accessibleHint: string option with get, set
            abstract tabIndex: float with get, set

    module CONST =

        type [<AllowNullLiteral>] IExports =
            abstract VERSION: string
            abstract PI_2: float
            abstract RAD_TO_DEG: float
            abstract DEG_TO_RAD: float
            abstract TARGET_FPMS: float
            abstract RENDERER_TYPE: obj
            abstract BLEND_MODES: obj
            abstract DRAW_MODES: obj
            abstract SCALE_MODES: obj
            abstract GC_MODES: obj
            abstract WRAP_MODES: obj
            abstract TRANSFORM_MODE: obj
            abstract URL_FILE_EXTENSION: U2<RegExp, string>
            abstract DATA_URI: U2<RegExp, string>
            abstract SVG_SIZE: U2<RegExp, string>
            abstract SHAPES: obj
            abstract PRECISION: obj
            abstract TEXT_GRADIENT: obj
            abstract UPDATE_PRIORITY: obj

    type [<AllowNullLiteral>] StageOptions =
        abstract children: bool option with get, set
        abstract texture: bool option with get, set
        abstract baseTexture: bool option with get, set

    type [<AllowNullLiteral>] Application =
        abstract _ticker: Ticker.Ticker with get, set
        abstract renderer: U2<PIXI.WebGLRenderer, PIXI.CanvasRenderer> with get, set
        abstract stage: Container with get, set
        abstract ticker: Ticker.Ticker with get, set
        abstract loader: Loaders.Loader with get, set
        abstract screen: Rectangle
        abstract stop: unit -> unit
        abstract start: unit -> unit
        abstract render: unit -> unit
        abstract destroy: ?removeView: bool * ?stageOptions: U2<StageOptions, bool> -> unit
        abstract view: HTMLCanvasElement

    type [<AllowNullLiteral>] ApplicationStatic =
        [<Emit "new $0($1...)">] abstract Create: ?options: ApplicationOptions -> Application
        [<Emit "new $0($1...)">] abstract Create: ?width: float * ?height: float * ?options: ApplicationOptions * ?noWebGL: bool * ?sharedTicker: bool * ?sharedLoader: bool -> Application

    type [<AllowNullLiteral>] DestroyOptions =
        abstract children: bool option with get, set
        abstract texture: bool option with get, set
        abstract baseTexture: bool option with get, set

    type [<AllowNullLiteral>] Bounds =
        abstract minX: float with get, set
        abstract minY: float with get, set
        abstract maxX: float with get, set
        abstract maxY: float with get, set
        abstract rect: Rectangle with get, set
        abstract isEmpty: unit -> bool
        abstract clear: unit -> unit
        abstract getRectangle: ?rect: Rectangle -> Rectangle
        abstract addPoint: point: Point -> unit
        abstract addQuad: vertices: ResizeArray<float> -> Bounds option
        abstract addFrame: transform: Transform * x0: float * y0: float * x1: float * y1: float -> unit
        abstract addVertices: transform: Transform * vertices: ResizeArray<float> * beginOffset: float * endOffset: float -> unit
        abstract addBounds: bounds: Bounds -> unit
        abstract addBoundsMask: bounds: Bounds * mask: Bounds -> unit
        abstract addBoundsArea: bounds: Bounds * area: Rectangle -> unit

    type [<AllowNullLiteral>] BoundsStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> Bounds

    type [<AllowNullLiteral>] Container =
        inherit DisplayObject
        abstract getChildByName: name: string -> DisplayObject
        abstract children: ResizeArray<DisplayObject> with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract onChildrenChange: (ResizeArray<obj option> -> unit) with get, set
        abstract addChild: child: 'T * [<ParamArray>] additionalChildren: ResizeArray<DisplayObject> -> 'T
        abstract addChildAt: child: 'T * index: float -> 'T
        abstract swapChildren: child: DisplayObject * child2: DisplayObject -> unit
        abstract getChildIndex: child: DisplayObject -> float
        abstract setChildIndex: child: DisplayObject * index: float -> unit
        abstract getChildAt: index: float -> DisplayObject
        abstract removeChild: child: DisplayObject -> DisplayObject
        abstract removeChildAt: index: float -> DisplayObject
        abstract removeChildren: ?beginIndex: float * ?endIndex: float -> ResizeArray<DisplayObject>
        abstract updateTransform: unit -> unit
        abstract calculateBounds: unit -> unit
        abstract _calculateBounds: unit -> unit
        abstract containerUpdateTransform: unit -> unit
        abstract renderWebGL: renderer: WebGLRenderer -> unit
        abstract renderAdvancedWebGL: renderer: WebGLRenderer -> unit
        abstract _renderWebGL: renderer: WebGLRenderer -> unit
        abstract _renderCanvas: renderer: CanvasRenderer -> unit
        abstract renderCanvas: renderer: CanvasRenderer -> unit
        abstract destroy: ?options: U2<DestroyOptions, bool> -> unit
        abstract once: ``event``: U2<string, string> * fn: (DisplayObject -> unit) * ?context: obj option -> Container
        abstract once: ``event``: string * fn: Function * ?context: obj option -> Container
        abstract on: ``event``: U2<string, string> * fn: (DisplayObject -> unit) * ?context: obj option -> Container
        abstract on: ``event``: string * fn: Function * ?context: obj option -> Container
        abstract off: ``event``: U3<string, string, string> * ?fn: Function * ?context: obj option -> Container

    type [<AllowNullLiteral>] ContainerStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> Container

    type [<AllowNullLiteral>] DisplayObject =
        inherit Utils.EventEmitter
        inherit Interaction.InteractiveTarget
        inherit Accessibility.AccessibleTarget
        abstract _cacheAsBitmap: bool with get, set
        abstract _cacheData: bool with get, set
        abstract cacheAsBitmap: bool with get, set
        abstract _renderCachedWebGL: renderer: WebGLRenderer -> unit
        abstract _initCachedDisplayObject: renderer: WebGLRenderer -> unit
        abstract _renderCachedCanvas: renderer: CanvasRenderer -> unit
        abstract _initCachedDisplayObjectCanvas: renderer: CanvasRenderer -> unit
        abstract _calculateCachedBounds: unit -> Rectangle
        abstract _getCachedLocalBounds: unit -> Rectangle
        abstract _destroyCachedDisplayObject: unit -> unit
        abstract _cacheAsBitmapDestroy: options: U2<bool, obj option> -> unit
        abstract name: string option with get, set
        abstract getGlobalPosition: ?point: Point * ?skipUpdate: bool -> Point
        abstract accessible: bool with get, set
        abstract accessibleTitle: string option with get, set
        abstract accessibleHint: string option with get, set
        abstract tabIndex: float with get, set
        abstract interactive: bool with get, set
        abstract interactiveChildren: bool with get, set
        abstract hitArea: U6<PIXI.Rectangle, PIXI.Circle, PIXI.Ellipse, PIXI.Polygon, PIXI.RoundedRectangle, PIXI.HitArea> with get, set
        abstract buttonMode: bool with get, set
        abstract cursor: string with get, set
        abstract trackedPointers: unit -> obj
        abstract defaultCursor: string with get, set
        abstract transform: TransformBase with get, set
        abstract alpha: float with get, set
        abstract visible: bool with get, set
        abstract renderable: bool with get, set
        abstract parent: Container with get, set
        abstract worldAlpha: float with get, set
        abstract filterArea: Rectangle option with get, set
        abstract _filters: Array<Filter<obj option>> option with get, set
        abstract _enabledFilters: Array<Filter<obj option>> option with get, set
        abstract _bounds: Bounds with get, set
        abstract _boundsID: float with get, set
        abstract _lastBoundsID: float with get, set
        abstract _boundsRect: Rectangle with get, set
        abstract _localBoundsRect: Rectangle with get, set
        abstract _mask: U2<PIXI.Graphics, PIXI.Sprite> option with get, set
        abstract _destroyed: bool
        abstract x: float with get, set
        abstract y: float with get, set
        abstract worldTransform: Matrix with get, set
        abstract localTransform: Matrix with get, set
        abstract position: U2<Point, ObservablePoint> with get, set
        abstract scale: U2<Point, ObservablePoint> with get, set
        abstract pivot: U2<Point, ObservablePoint> with get, set
        abstract skew: ObservablePoint with get, set
        abstract rotation: float with get, set
        abstract worldVisible: bool with get, set
        abstract mask: U2<PIXI.Graphics, PIXI.Sprite> option with get, set
        abstract filters: Array<Filter<obj option>> option with get, set
        abstract updateTransform: unit -> unit
        abstract displayObjectUpdateTransform: unit -> unit
        abstract _recursivePostUpdateTransform: unit -> unit
        abstract getBounds: ?skipUpdate: bool * ?rect: Rectangle -> Rectangle
        abstract getLocalBounds: ?rect: Rectangle -> Rectangle
        abstract toGlobal: position: PointLike -> Point
        abstract toGlobal: position: PointLike * ?point: 'T * ?skipUpdate: bool -> 'T
        abstract toLocal: position: PointLike * ?from: DisplayObject -> Point
        abstract toLocal: position: PointLike * ?from: DisplayObject * ?point: 'T * ?skipUpdate: bool -> 'T
        abstract renderWebGL: renderer: WebGLRenderer -> unit
        abstract renderCanvas: renderer: CanvasRenderer -> unit
        abstract setParent: container: Container -> Container
        abstract setTransform: ?x: float * ?y: float * ?scaleX: float * ?scaleY: float * ?rotation: float * ?skewX: float * ?skewY: float * ?pivotX: float * ?pivotY: float -> DisplayObject
        abstract destroy: unit -> unit
        abstract on: ``event``: Interaction.InteractionEventTypes * fn: (Interaction.InteractionEvent -> unit) * ?context: obj option -> DisplayObject
        abstract once: ``event``: Interaction.InteractionEventTypes * fn: (Interaction.InteractionEvent -> unit) * ?context: obj option -> DisplayObject
        abstract removeListener: ``event``: Interaction.InteractionEventTypes * ?fn: (Interaction.InteractionEvent -> unit) * ?context: obj option -> DisplayObject
        abstract removeAllListeners: ?``event``: Interaction.InteractionEventTypes -> DisplayObject
        abstract off: ``event``: Interaction.InteractionEventTypes * ?fn: (Interaction.InteractionEvent -> unit) * ?context: obj option -> DisplayObject
        abstract addListener: ``event``: Interaction.InteractionEventTypes * fn: (Interaction.InteractionEvent -> unit) * ?context: obj option -> DisplayObject

    type [<AllowNullLiteral>] DisplayObjectStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> DisplayObject

    type [<AllowNullLiteral>] TransformBase =
        abstract IDENTITY: TransformBase with get, set
        abstract worldTransform: Matrix with get, set
        abstract localTransform: Matrix with get, set
        abstract _worldID: float with get, set
        abstract updateLocalTransform: unit -> unit
        abstract updateTransform: parentTransform: TransformBase -> unit
        abstract updateWorldTransform: parentTransform: TransformBase -> unit

    type [<AllowNullLiteral>] TransformBaseStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> TransformBase

    type [<AllowNullLiteral>] Transform =
        inherit TransformBase
        abstract position: Point with get, set
        abstract scale: Point with get, set
        abstract skew: ObservablePoint with get, set
        abstract pivot: Point with get, set
        abstract _rotation: float with get, set
        abstract _sr: float option with get, set
        abstract _cr: float option with get, set
        abstract _cy: float option with get, set
        abstract _sy: float option with get, set
        abstract _sx: float option with get, set
        abstract _cx: float option with get, set
        abstract updateSkew: unit -> unit
        abstract setFromMatrix: matrix: Matrix -> unit
        abstract rotation: float with get, set
        [<Emit "new $0($1...)">] abstract Create: unit -> Transform

    type [<AllowNullLiteral>] GraphicsData =
        abstract lineWidth: float with get, set
        abstract lineAlignment: float with get, set
        abstract nativeLines: bool with get, set
        abstract lineColor: float with get, set
        abstract lineAlpha: float with get, set
        abstract _lineTint: float with get, set
        abstract fillColor: float with get, set
        abstract fillAlpha: float with get, set
        abstract _fillTint: float with get, set
        abstract fill: bool with get, set
        abstract holes: U6<ResizeArray<Circle>, ResizeArray<Rectangle>, ResizeArray<Ellipse>, ResizeArray<Polygon>, ResizeArray<RoundedRectangle>, ResizeArray<obj option>> with get, set
        abstract shape: U6<Circle, Rectangle, Ellipse, Polygon, RoundedRectangle, obj option> with get, set
        abstract ``type``: float option with get, set
        abstract clone: unit -> GraphicsData
        abstract addHole: shape: U6<Circle, Rectangle, Ellipse, Polygon, RoundedRectangle, obj option> -> unit
        abstract destroy: ?options: U2<DestroyOptions, bool> -> unit

    type [<AllowNullLiteral>] GraphicsDataStatic =
        [<Emit "new $0($1...)">] abstract Create: lineWidth: float * lineColor: float * lineAlpha: float * fillColor: float * fillAlpha: float * fill: bool * nativeLines: bool * shape: U6<Circle, Rectangle, Ellipse, Polygon, RoundedRectangle, obj option> * ?lineAlignment: float -> GraphicsData

    type [<AllowNullLiteral>] Graphics =
        inherit Container
        abstract CURVES: obj with get, set
        abstract fillAlpha: float with get, set
        abstract lineWidth: float with get, set
        abstract nativeLines: bool with get, set
        abstract lineColor: float with get, set
        abstract lineAlignment: float with get, set
        abstract graphicsData: ResizeArray<GraphicsData> with get, set
        abstract tint: float with get, set
        abstract _prevTint: float with get, set
        abstract blendMode: float with get, set
        abstract currentPath: GraphicsData with get, set
        abstract _webGL: obj option with get, set
        abstract isMask: bool with get, set
        abstract boundsPadding: float with get, set
        abstract _localBounds: Bounds with get, set
        abstract dirty: float with get, set
        abstract fastRectDirty: float with get, set
        abstract clearDirty: float with get, set
        abstract boundsDirty: float with get, set
        abstract cachedSpriteDirty: bool with get, set
        abstract _spriteRect: Rectangle with get, set
        abstract _fastRect: bool with get, set
        abstract _SPRITE_TEXTURE: Texture with get, set
        abstract clone: unit -> Graphics
        abstract _quadraticCurveLength: fromX: float * fromY: float * cpX: float * cpY: float * toX: float * toY: float -> float
        abstract _bezierCurveLength: fromX: float * fromY: float * cpX: float * cpY: float * cpX2: float * cpY2: float * toX: float * toY: float -> float
        abstract _segmentsCount: length: float -> float
        abstract lineStyle: ?lineWidth: float * ?color: float * ?alpha: float * ?alignment: float -> Graphics
        abstract moveTo: x: float * y: float -> Graphics
        abstract lineTo: x: float * y: float -> Graphics
        abstract quadraticCurveTo: cpX: float * cpY: float * toX: float * toY: float -> Graphics
        abstract bezierCurveTo: cpX: float * cpY: float * cpX2: float * cpY2: float * toX: float * toY: float -> Graphics
        abstract arcTo: x1: float * y1: float * x2: float * y2: float * radius: float -> Graphics
        abstract arc: cx: float * cy: float * radius: float * startAngle: float * endAngle: float * ?anticlockwise: bool -> Graphics
        abstract beginFill: color: float * ?alpha: float -> Graphics
        abstract endFill: unit -> Graphics
        abstract drawRect: x: float * y: float * width: float * height: float -> Graphics
        abstract drawRoundedRect: x: float * y: float * width: float * height: float * radius: float -> Graphics
        abstract drawCircle: x: float * y: float * radius: float -> Graphics
        abstract drawEllipse: x: float * y: float * width: float * height: float -> Graphics
        abstract drawPolygon: path: U3<ResizeArray<float>, ResizeArray<Point>, Polygon> -> Graphics
        abstract drawStar: x: float * y: float * points: float * radius: float * innerRadius: float * ?rotation: float -> Graphics
        abstract clear: unit -> Graphics
        abstract isFastRect: unit -> bool
        abstract _renderCanvas: renderer: CanvasRenderer -> unit
        abstract _calculateBounds: unit -> Rectangle
        abstract _renderSpriteRect: renderer: PIXI.SystemRenderer -> unit
        abstract containsPoint: point: Point -> bool
        abstract updateLocalBounds: unit -> unit
        abstract drawShape: shape: U6<Circle, Rectangle, Ellipse, Polygon, RoundedRectangle, obj option> -> GraphicsData
        abstract generateCanvasTexture: ?scaleMode: float * ?resolution: float -> Texture
        abstract closePath: unit -> Graphics
        abstract addHole: unit -> Graphics
        abstract destroy: ?options: U2<DestroyOptions, bool> -> unit

    type [<AllowNullLiteral>] GraphicsStatic =
        [<Emit "new $0($1...)">] abstract Create: ?nativeLines: bool -> Graphics

    type [<AllowNullLiteral>] CanvasGraphicsRenderer =
        abstract render: graphics: Graphics -> unit
        abstract updateGraphicsTint: graphics: Graphics -> unit
        abstract renderPolygon: points: ResizeArray<Point> * close: bool * context: CanvasRenderingContext2D -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] CanvasGraphicsRendererStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: SystemRenderer -> CanvasGraphicsRenderer

    type [<AllowNullLiteral>] GraphicsRenderer =
        inherit ObjectRenderer
        abstract graphicsDataPool: ResizeArray<GraphicsData> with get, set
        abstract primitiveShader: PrimitiveShader with get, set
        abstract gl: WebGLRenderingContext with get, set
        abstract CONTEXT_UID: float with get, set
        abstract destroy: unit -> unit
        abstract render: graphics: Graphics -> unit
        abstract updateGraphics: graphics: PIXI.Graphics -> unit
        abstract getWebGLData: webGL: WebGLRenderingContext * ``type``: float * nativeLines: float -> WebGLGraphicsData

    type [<AllowNullLiteral>] GraphicsRendererStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: PIXI.CanvasRenderer -> GraphicsRenderer

    type [<AllowNullLiteral>] WebGLGraphicsData =
        abstract gl: WebGLRenderingContext with get, set
        abstract color: ResizeArray<float> with get, set
        abstract points: ResizeArray<Point> with get, set
        abstract indices: ResizeArray<float> with get, set
        abstract buffer: WebGLBuffer with get, set
        abstract indexBuffer: WebGLBuffer with get, set
        abstract dirty: bool with get, set
        abstract glPoints: ResizeArray<float> with get, set
        abstract glIndices: ResizeArray<float> with get, set
        abstract shader: GlCore.GLShader with get, set
        abstract vao: GlCore.VertexArrayObject with get, set
        abstract nativeLines: bool with get, set
        abstract reset: unit -> unit
        abstract upload: unit -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] WebGLGraphicsDataStatic =
        [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * shader: GlCore.GLShader * attribsState: GlCore.AttribState -> WebGLGraphicsData

    type [<AllowNullLiteral>] PrimitiveShader =
        inherit GlCore.GLShader

    type [<AllowNullLiteral>] PrimitiveShaderStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> PrimitiveShader

    module GroupD8 =

        type [<AllowNullLiteral>] IExports =
            abstract E: float
            abstract SE: float
            abstract S: float
            abstract SW: float
            abstract W: float
            abstract NW: float
            abstract N: float
            abstract NE: float
            abstract MIRROR_HORIZONTAL: float
            abstract MIRROR_VERTICAL: float
            abstract uX: ind: float -> float
            abstract uY: ind: float -> float
            abstract vX: ind: float -> float
            abstract vY: ind: float -> float
            abstract inv: rotation: float -> float
            abstract add: rotationSecond: float * rotationFirst: float -> float
            abstract sub: rotationSecond: float * rotationFirst: float -> float
            abstract rotate180: rotation: float -> float
            abstract isVertical: rotation: float -> bool
            abstract byDirection: dx: float * dy: float -> float
            abstract matrixAppendRotationInv: matrix: Matrix * rotation: float * tx: float * ty: float -> unit
            abstract isSwapWidthHeight: rotation: float -> bool

    type [<AllowNullLiteral>] Matrix =
        abstract a: float with get, set
        abstract b: float with get, set
        abstract c: float with get, set
        abstract d: float with get, set
        abstract tx: float with get, set
        abstract ty: float with get, set
        abstract fromArray: array: ResizeArray<float> -> unit
        abstract set: a: float * b: float * c: float * d: float * tx: float * ty: float -> Matrix
        abstract toArray: ?transpose: bool * ?out: ResizeArray<float> -> ResizeArray<float>
        abstract apply: pos: Point * ?newPos: Point -> Point
        abstract applyInverse: pos: Point * ?newPos: Point -> Point
        abstract translate: x: float * y: float -> Matrix
        abstract scale: x: float * y: float -> Matrix
        abstract rotate: angle: float -> Matrix
        abstract append: matrix: Matrix -> Matrix
        abstract setTransform: x: float * y: float * pivotX: float * pivotY: float * scaleX: float * scaleY: float * rotation: float * skewX: float * skewY: float -> PIXI.Matrix
        abstract prepend: matrix: Matrix -> Matrix
        abstract invert: unit -> Matrix
        abstract identity: unit -> Matrix
        abstract decompose: transform: TransformBase -> TransformBase
        abstract clone: unit -> Matrix
        abstract copy: matrix: Matrix -> Matrix
        abstract IDENTITY: Matrix with get, set
        abstract TEMP_MATRIX: Matrix with get, set

    type [<AllowNullLiteral>] MatrixStatic =
        [<Emit "new $0($1...)">] abstract Create: ?a: float * ?b: float * ?c: float * ?d: float * ?tx: float * ?ty: float -> Matrix

    type [<AllowNullLiteral>] PointLike =
        abstract x: float with get, set
        abstract y: float with get, set
        abstract set: ?x: float * ?y: float -> unit
        abstract copy: point: PointLike -> unit

    type [<AllowNullLiteral>] PointLikeStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> PointLike

    type [<AllowNullLiteral>] ObservablePoint =
        inherit PointLike
        abstract cb: (unit -> obj option) with get, set
        abstract scope: obj option with get, set

    type [<AllowNullLiteral>] ObservablePointStatic =
        [<Emit "new $0($1...)">] abstract Create: cb: (unit -> obj option) * ?scope: obj option * ?x: float * ?y: float -> ObservablePoint

    type [<AllowNullLiteral>] Point =
        inherit PointLike
        abstract clone: unit -> Point
        abstract equals: p: PointLike -> bool

    type [<AllowNullLiteral>] PointStatic =
        [<Emit "new $0($1...)">] abstract Create: ?x: float * ?y: float -> Point

    type [<AllowNullLiteral>] HitArea =
        abstract contains: x: float * y: float -> bool

    type [<AllowNullLiteral>] Circle =
        inherit HitArea
        abstract x: float with get, set
        abstract y: float with get, set
        abstract radius: float with get, set
        abstract ``type``: float with get, set
        abstract clone: unit -> Circle
        abstract contains: x: float * y: float -> bool
        abstract getBounds: unit -> Rectangle

    type [<AllowNullLiteral>] CircleStatic =
        [<Emit "new $0($1...)">] abstract Create: ?x: float * ?y: float * ?radius: float -> Circle

    type [<AllowNullLiteral>] Ellipse =
        inherit HitArea
        abstract x: float with get, set
        abstract y: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract ``type``: float with get, set
        abstract clone: unit -> Ellipse
        abstract contains: x: float * y: float -> bool
        abstract getBounds: unit -> Rectangle

    type [<AllowNullLiteral>] EllipseStatic =
        [<Emit "new $0($1...)">] abstract Create: ?x: float * ?y: float * ?width: float * ?height: float -> Ellipse

    type [<AllowNullLiteral>] Polygon =
        inherit HitArea
        abstract closed: bool with get, set
        abstract points: ResizeArray<float> with get, set
        abstract ``type``: float with get, set
        abstract clone: unit -> Polygon
        abstract contains: x: float * y: float -> bool
        abstract close: unit -> unit

    type [<AllowNullLiteral>] PolygonStatic =
        [<Emit "new $0($1...)">] abstract Create: points: U2<ResizeArray<Point>, ResizeArray<float>> -> Polygon
        [<Emit "new $0($1...)">] abstract Create: [<ParamArray>] points: ResizeArray<Point> -> Polygon
        [<Emit "new $0($1...)">] abstract Create: [<ParamArray>] points: ResizeArray<float> -> Polygon

    type [<AllowNullLiteral>] Rectangle =
        inherit HitArea
        abstract x: float with get, set
        abstract y: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract ``type``: float with get, set
        abstract left: float with get, set
        abstract right: float with get, set
        abstract top: float with get, set
        abstract bottom: float with get, set
        abstract EMPTY: Rectangle with get, set
        abstract clone: unit -> Rectangle
        abstract copy: rectangle: Rectangle -> Rectangle
        abstract contains: x: float * y: float -> bool
        abstract pad: paddingX: float * paddingY: float -> unit
        abstract fit: rectangle: Rectangle -> unit
        abstract enlarge: rectangle: Rectangle -> unit

    type [<AllowNullLiteral>] RectangleStatic =
        [<Emit "new $0($1...)">] abstract Create: ?x: float * ?y: float * ?width: float * ?height: float -> Rectangle

    type [<AllowNullLiteral>] RoundedRectangle =
        inherit HitArea
        abstract x: float with get, set
        abstract y: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract radius: float with get, set
        abstract ``type``: float with get, set
        abstract clone: unit -> RoundedRectangle
        abstract contains: x: float * y: float -> bool

    type [<AllowNullLiteral>] RoundedRectangleStatic =
        [<Emit "new $0($1...)">] abstract Create: ?x: float * ?y: float * ?width: float * ?height: float * ?radius: float -> RoundedRectangle

    type [<AllowNullLiteral>] RendererOptions =
        /// the width of the renderers view [default=800]
        abstract width: float option with get, set
        /// the height of the renderers view [default=600]
        abstract height: float option with get, set
        /// the canvas to use as a view, optional
        abstract view: HTMLCanvasElement option with get, set
        /// If the render view is transparent, [default=false]
        abstract transparent: bool option with get, set
        /// sets antialias (only applicable in chrome at the moment) [default=false]
        abstract antialias: bool option with get, set
        /// enables drawing buffer preservation, enable this if you need to call toDataUrl on the webgl context [default=false]
        abstract preserveDrawingBuffer: bool option with get, set
        /// The resolution / device pixel ratio of the renderer, retina would be 2 [default=1]
        abstract resolution: float option with get, set
        /// prevents selection of WebGL renderer, even if such is present [default=false]
        abstract forceCanvas: bool option with get, set
        /// The background color of the rendered area (shown if not transparent) [default=0x000000]
        abstract backgroundColor: float option with get, set
        /// This sets if the renderer will clear the canvas or not before the new render pass. [default=true]
        abstract clearBeforeRender: bool option with get, set
        /// If true Pixi will Math.floor() x/ y values when rendering, stopping pixel interpolation. [default=false]
        abstract roundPixels: bool option with get, set
        /// forces FXAA antialiasing to be used over native FXAA is faster, but may not always look as great ** webgl only** [default=false]
        abstract forceFXAA: bool option with get, set
        /// `true` to ensure compatibility with older / less advanced devices. If you experience unexplained flickering try setting this to true. **webgl only** [default=false]
        abstract legacy: bool option with get, set
        /// Deprecated
        abstract context: WebGLRenderingContext option with get, set
        /// Deprecated
        abstract autoResize: bool option with get, set
        /// Parameter passed to webgl context, set to "high-performance" for devices with dual graphics card
        abstract powerPreference: string option with get, set

    type [<AllowNullLiteral>] ApplicationOptions =
        inherit RendererOptions
        /// `true` to use PIXI.ticker.shared, `false` to create new ticker. [default=false]
        abstract sharedTicker: bool option with get, set
        /// `true` to use PIXI.loaders.shared, `false` to create new Loader.
        abstract sharedLoader: bool option with get, set
        /// automatically starts the rendering after the construction.
        /// Note that setting this parameter to false does NOT stop the shared ticker even if you set
        /// options.sharedTicker to true in case that it is already started. Stop it by your own.
        abstract autoStart: bool option with get, set

    type [<AllowNullLiteral>] DefaultRendererPlugins =
        abstract accessibility: Accessibility.AccessibilityManager with get, set
        abstract interaction: Interaction.InteractionManager with get, set

    type [<AllowNullLiteral>] RendererPlugins =
        inherit DefaultRendererPlugins

    type [<AllowNullLiteral>] SystemRenderer =
        inherit Utils.EventEmitter
        abstract ``type``: float with get, set
        abstract options: RendererOptions with get, set
        abstract screen: Rectangle with get, set
        abstract width: float
        abstract height: float
        abstract view: HTMLCanvasElement with get, set
        abstract resolution: float with get, set
        abstract transparent: bool with get, set
        abstract autoResize: bool with get, set
        abstract blendModes: obj option with get, set
        abstract preserveDrawingBuffer: bool with get, set
        abstract clearBeforeRender: bool with get, set
        abstract roundPixels: bool with get, set
        abstract backgroundColor: float with get, set
        abstract _backgroundColor: float with get, set
        abstract _backgroundColorRgba: ResizeArray<float> with get, set
        abstract _backgroundColorString: string with get, set
        abstract _tempDisplayObjectParent: Container with get, set
        abstract _lastObjectRendered: DisplayObject with get, set
        abstract resize: screenWidth: float * screenHeight: float -> unit
        abstract generateTexture: displayObject: DisplayObject * ?scaleMode: float * ?resolution: float * ?region: Rectangle -> RenderTexture
        abstract render: [<ParamArray>] args: ResizeArray<obj option> -> unit
        abstract destroy: ?removeView: bool -> unit

    type [<AllowNullLiteral>] SystemRendererStatic =
        [<Emit "new $0($1...)">] abstract Create: system: string * ?options: RendererOptions -> SystemRenderer
        [<Emit "new $0($1...)">] abstract Create: system: string * ?screenWidth: float * ?screenHeight: float * ?options: RendererOptions -> SystemRenderer

    type [<AllowNullLiteral>] DefaultCanvasRendererPlugins =
        abstract extract: Extract.CanvasExtract with get, set
        abstract prepare: Prepare.CanvasPrepare with get, set

    type [<AllowNullLiteral>] CanvasRendererPlugins =
        inherit DefaultCanvasRendererPlugins
        inherit RendererPlugins

    type [<AllowNullLiteral>] CanvasRenderer =
        inherit SystemRenderer
        abstract __plugins: obj with get, set
        abstract plugins: obj option with get, set
        abstract initPlugins: unit -> unit
        abstract destroyPlugins: unit -> unit
        abstract _activeBlendMode: float with get, set
        abstract rootContext: CanvasRenderingContext2D with get, set
        abstract rootResolution: float option with get, set
        abstract refresh: bool with get, set
        abstract maskManager: CanvasMaskManager with get, set
        abstract smoothProperty: string with get, set
        abstract extract: Extract.CanvasExtract with get, set
        abstract context: CanvasRenderingContext2D option with get, set
        abstract render: displayObject: PIXI.DisplayObject * ?renderTexture: PIXI.RenderTexture * ?clear: bool * ?transform: PIXI.Transform * ?skipUpdateTransform: bool -> unit
        abstract setBlendMode: blendMode: float -> unit
        abstract destroy: ?removeView: bool -> unit
        abstract clear: ?clearColor: string -> unit
        abstract invalidateBlendMode: unit -> unit
        abstract on: ``event``: U2<string, string> * fn: (unit -> unit) * ?context: obj option -> CanvasRenderer
        abstract once: ``event``: U2<string, string> * fn: (unit -> unit) * ?context: obj option -> CanvasRenderer
        abstract removeListener: ``event``: U2<string, string> * ?fn: (unit -> unit) * ?context: obj option -> CanvasRenderer
        abstract removeAllListeners: ?``event``: U2<string, string> -> CanvasRenderer
        abstract off: ``event``: U2<string, string> * ?fn: (unit -> unit) * ?context: obj option -> CanvasRenderer
        abstract addListener: ``event``: U2<string, string> * fn: (unit -> unit) * ?context: obj option -> CanvasRenderer

    type [<AllowNullLiteral>] CanvasRendererStatic =
        abstract registerPlugin: pluginName: string * ctor: CanvasRendererStaticRegisterPluginCtor -> unit
        [<Emit "new $0($1...)">] abstract Create: ?options: RendererOptions -> CanvasRenderer
        [<Emit "new $0($1...)">] abstract Create: ?screenWidth: float * ?screenHeight: float * ?options: RendererOptions -> CanvasRenderer

    type [<AllowNullLiteral>] CanvasRendererStaticRegisterPluginCtor =
        [<Emit "new $0($1...)">] abstract Create: renderer: CanvasRenderer -> CanvasRenderer

    type [<AllowNullLiteral>] CanvasMaskManager =
        abstract pushMask: maskData: obj option -> unit
        abstract renderGraphicsShape: graphics: Graphics -> unit
        abstract popMask: renderer: U2<WebGLRenderer, CanvasRenderer> -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] CanvasMaskManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: CanvasRenderer -> CanvasMaskManager

    type [<AllowNullLiteral>] CanvasRenderTarget =
        abstract canvas: HTMLCanvasElement with get, set
        abstract context: CanvasRenderingContext2D with get, set
        abstract resolution: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract clear: unit -> unit
        abstract resize: width: float * height: float -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] CanvasRenderTargetStatic =
        [<Emit "new $0($1...)">] abstract Create: width: float * height: float * resolution: float -> CanvasRenderTarget

    type [<AllowNullLiteral>] WebGLRendererOptions =
        inherit RendererOptions

    type [<AllowNullLiteral>] DefaultWebGLRendererPlugins =
        abstract extract: Extract.WebGLExtract with get, set
        abstract prepare: Prepare.WebGLPrepare with get, set

    type [<AllowNullLiteral>] WebGLRendererPlugins =
        inherit DefaultWebGLRendererPlugins
        inherit RendererPlugins

    type [<AllowNullLiteral>] WebGLRenderer =
        inherit SystemRenderer
        abstract __plugins: obj with get, set
        abstract plugins: obj option with get, set
        abstract initPlugins: unit -> unit
        abstract destroyPlugins: unit -> unit
        abstract _contextOptions: obj with get, set
        abstract _backgroundColorRgba: ResizeArray<float> with get, set
        abstract maskManager: MaskManager with get, set
        abstract stencilManager: StencilManager option with get, set
        abstract emptyRenderer: ObjectRenderer with get, set
        abstract currentRenderer: ObjectRenderer with get, set
        abstract gl: WebGLRenderingContext with get, set
        abstract CONTEXT_UID: float with get, set
        abstract state: WebGLState option with get, set
        abstract renderingToScreen: bool with get, set
        abstract boundTextures: ResizeArray<BaseTexture> with get, set
        abstract filterManager: FilterManager with get, set
        abstract textureManager: TextureManager option with get, set
        abstract textureGC: TextureGarbageCollector option with get, set
        abstract extract: Extract.WebGLExtract with get, set
        abstract drawModes: obj option with get, set
        abstract _activeShader: Shader with get, set
        abstract _activeRenderTarget: RenderTarget with get, set
        abstract _initContext: unit -> unit
        abstract render: displayObject: PIXI.DisplayObject * ?renderTexture: PIXI.RenderTexture * ?clear: bool * ?transform: PIXI.Transform * ?skipUpdateTransform: bool -> unit
        abstract setObjectRenderer: objectRenderer: ObjectRenderer -> unit
        abstract flush: unit -> unit
        abstract setBlendMode: blendMode: float -> unit
        abstract clear: ?clearColor: float -> unit
        abstract setTransform: matrix: Matrix -> unit
        abstract clearRenderTexture: renderTexture: RenderTexture * ?clearColor: float -> WebGLRenderer
        abstract bindRenderTexture: renderTexture: RenderTexture * transform: Transform -> WebGLRenderer
        abstract bindRenderTarget: renderTarget: RenderTarget -> WebGLRenderer
        abstract bindShader: shader: Shader * ?autoProject: bool -> WebGLRenderer
        abstract bindTexture: texture: U2<Texture, BaseTexture> * ?location: float * ?forceLocation: bool -> float
        abstract unbindTexture: texture: U2<Texture, BaseTexture> -> WebGLRenderer option
        abstract createVao: unit -> GlCore.VertexArrayObject
        abstract bindVao: vao: GlCore.VertexArrayObject -> WebGLRenderer
        abstract reset: unit -> WebGLRenderer
        abstract handleContextLost: (WebGLContextEvent -> unit) with get, set
        abstract handleContextRestored: (unit -> unit) with get, set
        abstract destroy: ?removeView: bool -> unit
        abstract on: ``event``: U2<string, string> * fn: (unit -> unit) * ?context: obj option -> WebGLRenderer
        [<Emit "$0.on('context',$1,$2)">] abstract on_context: fn: (WebGLRenderingContext -> unit) * ?context: obj option -> WebGLRenderer
        abstract once: ``event``: U2<string, string> * fn: (unit -> unit) * ?context: obj option -> WebGLRenderer
        [<Emit "$0.once('context',$1,$2)">] abstract once_context: fn: (WebGLRenderingContext -> unit) * ?context: obj option -> WebGLRenderer
        abstract removeListener: ``event``: U2<string, string> * ?fn: (unit -> unit) * ?context: obj option -> WebGLRenderer
        [<Emit "$0.removeListener('context',$1,$2)">] abstract removeListener_context: ?fn: (WebGLRenderingContext -> unit) * ?context: obj option -> WebGLRenderer
        abstract removeAllListeners: ?``event``: U3<string, string, string> -> WebGLRenderer
        abstract off: ``event``: U2<string, string> * ?fn: (unit -> unit) * ?context: obj option -> WebGLRenderer
        [<Emit "$0.off('context',$1,$2)">] abstract off_context: ?fn: (WebGLRenderingContext -> unit) * ?context: obj option -> WebGLRenderer
        abstract addListener: ``event``: U2<string, string> * fn: (unit -> unit) * ?context: obj option -> WebGLRenderer
        [<Emit "$0.addListener('context',$1,$2)">] abstract addListener_context: fn: (WebGLRenderingContext -> unit) * ?context: obj option -> WebGLRenderer

    type [<AllowNullLiteral>] WebGLRendererStatic =
        abstract registerPlugin: pluginName: string * ctor: WebGLRendererStaticRegisterPluginCtor -> unit
        [<Emit "new $0($1...)">] abstract Create: ?options: WebGLRendererOptions -> WebGLRenderer
        [<Emit "new $0($1...)">] abstract Create: ?screenWidth: float * ?screenHeight: float * ?options: WebGLRendererOptions -> WebGLRenderer

    type [<AllowNullLiteral>] WebGLRendererStaticRegisterPluginCtor =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> WebGLRenderer

    type [<AllowNullLiteral>] WebGLState =
        abstract activeState: ResizeArray<float> with get, set
        abstract defaultState: ResizeArray<float> with get, set
        abstract stackIndex: float with get, set
        abstract stack: ResizeArray<float> with get, set
        abstract gl: WebGLRenderingContext with get, set
        abstract maxAttribs: float with get, set
        abstract attribState: GlCore.AttribState with get, set
        abstract nativeVaoExtension: obj option with get, set
        abstract push: unit -> unit
        abstract pop: unit -> unit
        abstract setState: state: ResizeArray<float> -> unit
        abstract setBlend: value: float -> unit
        abstract setBlendMode: value: float -> unit
        abstract setDepthTest: value: float -> unit
        abstract setCullFace: value: float -> unit
        abstract setFrontFace: value: float -> unit
        abstract resetAttributes: unit -> unit
        abstract resetToDefault: unit -> unit

    type [<AllowNullLiteral>] WebGLStateStatic =
        [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext -> WebGLState

    type [<AllowNullLiteral>] TextureManager =
        abstract renderer: WebGLRenderer with get, set
        abstract gl: WebGLRenderingContext with get, set
        abstract _managedTextures: ResizeArray<Texture> with get, set
        abstract bindTexture: unit -> unit
        abstract getTexture: unit -> WebGLTexture
        abstract updateTexture: texture: U2<BaseTexture, Texture> -> WebGLTexture
        abstract destroyTexture: texture: BaseTexture * ?_skipRemove: bool -> unit
        abstract removeAll: unit -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] TextureManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> TextureManager

    type [<AllowNullLiteral>] TextureGarbageCollector =
        abstract renderer: WebGLRenderer with get, set
        abstract count: float with get, set
        abstract checkCount: float with get, set
        abstract maxIdle: float with get, set
        abstract checkCountMax: float with get, set
        abstract mode: float with get, set
        abstract update: unit -> unit
        abstract run: unit -> unit
        abstract unload: displayObject: DisplayObject -> unit

    type [<AllowNullLiteral>] TextureGarbageCollectorStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> TextureGarbageCollector

    type [<AllowNullLiteral>] ObjectRenderer =
        inherit WebGLManager
        abstract start: unit -> unit
        abstract stop: unit -> unit
        abstract flush: unit -> unit
        abstract render: [<ParamArray>] args: ResizeArray<obj option> -> unit

    type [<AllowNullLiteral>] ObjectRendererStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> ObjectRenderer

    type [<AllowNullLiteral>] Quad =
        abstract gl: WebGLRenderingContext with get, set
        abstract vertices: ResizeArray<float> with get, set
        abstract uvs: ResizeArray<float> with get, set
        abstract interleaved: ResizeArray<float> with get, set
        abstract indices: ResizeArray<float> with get, set
        abstract vertexBuffer: WebGLBuffer with get, set
        abstract vao: GlCore.VertexArrayObject with get, set
        abstract initVao: shader: GlCore.GLShader -> unit
        abstract map: targetTextureFrame: Rectangle * destinationFrame: Rectangle -> Quad
        abstract upload: unit -> Quad
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] QuadStatic =
        [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext -> Quad

    type [<AllowNullLiteral>] FilterDataStackItem =
        abstract renderTarget: RenderTarget with get, set
        abstract filter: ResizeArray<obj option> with get, set
        abstract bounds: Rectangle with get, set

    type [<AllowNullLiteral>] RenderTarget =
        abstract filterPoolKey: string with get, set
        abstract gl: WebGLRenderingContext with get, set
        abstract frameBuffer: GlCore.GLFramebuffer with get, set
        abstract texture: Texture with get, set
        abstract clearColor: ResizeArray<float> with get, set
        abstract size: Rectangle with get, set
        abstract resolution: float with get, set
        abstract projectionMatrix: Matrix with get, set
        abstract transform: Matrix with get, set
        abstract frame: Rectangle with get, set
        abstract defaultFrame: Rectangle with get, set
        abstract destinationFrame: Rectangle with get, set
        abstract sourceFrame: Rectangle option with get, set
        abstract stencilBuffer: GlCore.GLFramebuffer with get, set
        abstract stencilMaskStack: ResizeArray<Graphics> with get, set
        abstract filterData: obj with get, set
        abstract scaleMode: float with get, set
        abstract root: bool with get, set
        abstract clear: ?clearColor: ResizeArray<float> -> unit
        abstract attachStencilBuffer: unit -> unit
        abstract setFrame: destinationFrame: Rectangle * sourceFrame: Rectangle -> unit
        abstract activate: unit -> unit
        abstract calculateProjection: destinationFrame: Rectangle * sourceFrame: Rectangle -> unit
        abstract resize: width: float * height: float -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] RenderTargetStatic =
        [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * width: float * height: float * scaleMode: float * resolution: float * ?root: bool -> RenderTarget

    type [<AllowNullLiteral>] BlendModeManager =
        inherit WebGLManager
        abstract currentBlendMode: float with get, set
        abstract setBlendMode: blendMode: float -> bool

    type [<AllowNullLiteral>] BlendModeManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> BlendModeManager

    type [<AllowNullLiteral>] FilterManagerStackItem =
        abstract renderTarget: RenderTarget with get, set
        abstract sourceFrame: Rectangle with get, set
        abstract destinationFrame: Rectangle with get, set
        abstract filters: Array<Filter<obj option>> with get, set
        abstract target: obj option with get, set
        abstract resolution: float with get, set

    type [<AllowNullLiteral>] FilterManager =
        inherit WebGLManager
        abstract _screenWidth: float with get, set
        abstract _screenHeight: float with get, set
        abstract gl: WebGLRenderingContext with get, set
        abstract quad: Quad with get, set
        abstract stack: ResizeArray<FilterManagerStackItem> with get, set
        abstract stackIndex: float with get, set
        abstract shaderCache: obj option with get, set
        abstract filterData: obj option with get, set
        abstract onPrerender: unit -> unit
        abstract pushFilter: target: RenderTarget * filters: Array<Filter<obj option>> -> unit
        abstract popFilter: unit -> unit
        abstract applyFilter: shader: U2<GlCore.GLShader, Filter<obj option>> * inputTarget: RenderTarget * outputTarget: RenderTarget * ?clear: bool -> unit
        abstract syncUniforms: shader: GlCore.GLShader * filter: Filter<obj option> -> unit
        abstract getRenderTarget: ?clear: bool * ?resolution: float -> RenderTarget
        abstract returnRenderTarget: renderTarget: RenderTarget -> RenderTarget
        abstract calculateScreenSpaceMatrix: outputMatrix: Matrix -> Matrix
        abstract calculateNormalizedScreenSpaceMatrix: outputMatrix: Matrix -> Matrix
        abstract calculateSpriteMatrix: outputMatrix: Matrix * sprite: Sprite -> Matrix
        abstract destroy: ?contextLost: bool -> unit
        abstract emptyPool: unit -> unit
        abstract getPotRenderTarget: gl: WebGLRenderingContext * minWidth: float * minHeight: float * resolution: float -> RenderTarget
        abstract freePotRenderTarget: renderTarget: RenderTarget -> unit

    type [<AllowNullLiteral>] FilterManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> FilterManager

    type [<AllowNullLiteral>] StencilMaskStack =
        abstract stencilStack: ResizeArray<obj option> with get, set
        abstract reverse: bool with get, set
        abstract count: float with get, set

    type [<AllowNullLiteral>] StencilMaskStackStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> StencilMaskStack

    type [<AllowNullLiteral>] MaskManager =
        inherit WebGLManager
        abstract scissor: bool with get, set
        abstract scissorData: obj option with get, set
        abstract scissorRenderTarget: RenderTarget with get, set
        abstract enableScissor: bool with get, set
        abstract alphaMaskPool: ResizeArray<float> with get, set
        abstract alphaMaskIndex: float with get, set
        abstract pushMask: target: RenderTarget * maskData: U2<Sprite, Graphics> -> unit
        abstract popMask: target: RenderTarget * maskData: U2<Sprite, Graphics> -> unit
        abstract pushSpriteMask: target: RenderTarget * maskData: U2<Sprite, Graphics> -> unit
        abstract popSpriteMask: unit -> unit
        abstract pushStencilMask: maskData: U2<Sprite, Graphics> -> unit
        abstract popStencilMask: unit -> unit
        abstract pushScissorMask: target: RenderTarget * maskData: U2<Sprite, Graphics> -> unit
        abstract popScissorMask: unit -> unit

    type [<AllowNullLiteral>] MaskManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> MaskManager

    type [<AllowNullLiteral>] StencilManager =
        inherit WebGLManager
        abstract stencilMaskStack: ResizeArray<Graphics> with get, set
        abstract _useCurrent: unit -> unit
        abstract _getBitwiseMask: unit -> float
        abstract setMaskStack: stencilMasStack: ResizeArray<Graphics> -> unit
        abstract pushStencil: graphics: Graphics -> unit
        abstract popStencil: unit -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] StencilManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> StencilManager

    type [<AllowNullLiteral>] WebGLManager =
        abstract renderer: WebGLRenderer with get, set
        abstract onContextChange: unit -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] WebGLManagerStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> WebGLManager

    type [<AllowNullLiteral>] UniformData<'V> =
        abstract ``type``: string with get, set
        abstract value: 'V with get, set
        abstract name: string option with get, set

    type [<AllowNullLiteral>] UniformDataMap<'U> =
        interface end

    type [<AllowNullLiteral>] Filter<'U> =
        abstract _blendMode: float with get, set
        abstract vertexSrc: string option with get, set
        abstract fragmentSrc: string with get, set
        abstract blendMode: float with get, set
        abstract uniformData: UniformDataMap<'U> with get, set
        abstract uniforms: 'U with get, set
        abstract glShaders: obj option with get, set
        abstract glShaderKey: float option with get, set
        abstract padding: float with get, set
        abstract resolution: float with get, set
        abstract enabled: bool with get, set
        abstract autoFit: bool with get, set
        abstract apply: filterManager: FilterManager * input: RenderTarget * output: RenderTarget * ?clear: bool * ?currentState: obj option -> unit
        abstract defaultVertexSrc: string with get, set
        abstract defaultFragmentSrc: string with get, set

    type [<AllowNullLiteral>] FilterStatic =
        [<Emit "new $0($1...)">] abstract Create: ?vertexSrc: string * ?fragmentSrc: string * ?uniforms: UniformDataMap<'U> -> Filter<'U>

    type [<AllowNullLiteral>] SpriteMaskFilterUniforms =
        abstract mask: Texture with get, set
        abstract otherMatrix: Matrix with get, set
        abstract alpha: float with get, set

    type [<AllowNullLiteral>] SpriteMaskFilter =
        inherit Filter<SpriteMaskFilterUniforms>
        abstract maskSprite: Sprite with get, set
        abstract maskMatrix: Matrix with get, set
        abstract apply: filterManager: FilterManager * input: RenderTarget * output: RenderTarget -> unit

    type [<AllowNullLiteral>] SpriteMaskFilterStatic =
        [<Emit "new $0($1...)">] abstract Create: sprite: Sprite -> SpriteMaskFilter

    type [<AllowNullLiteral>] Sprite =
        inherit Container
        abstract _anchor: ObservablePoint with get, set
        abstract anchor: ObservablePoint with get, set
        abstract _texture: Texture with get, set
        abstract _transformTrimmedID: float with get, set
        abstract _textureTrimmedID: float with get, set
        abstract _width: float with get, set
        abstract _height: float with get, set
        abstract tint: float with get, set
        abstract _tint: float with get, set
        abstract _tintRGB: float with get, set
        abstract blendMode: float with get, set
        abstract pluginName: string with get, set
        abstract cachedTint: float with get, set
        abstract texture: Texture with get, set
        abstract textureDirty: bool with get, set
        abstract _textureID: float with get, set
        abstract _transformID: float with get, set
        abstract vertexTrimmedData: Float32Array with get, set
        abstract vertexData: Float32Array with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract _onTextureUpdate: unit -> unit
        abstract calculateVertices: unit -> unit
        abstract _calculateBounds: unit -> unit
        abstract calculateTrimmedVertices: unit -> unit
        abstract onAnchorUpdate: unit -> unit
        abstract _renderWebGL: renderer: WebGLRenderer -> unit
        abstract _renderCanvas: renderer: CanvasRenderer -> unit
        abstract getLocalBounds: unit -> Rectangle
        abstract containsPoint: point: Point -> bool
        abstract destroy: ?options: U2<DestroyOptions, bool> -> unit

    type [<AllowNullLiteral>] SpriteStatic =
        [<Emit "new $0($1...)">] abstract Create: ?texture: Texture -> Sprite
        abstract from: source: U6<float, string, BaseTexture, HTMLImageElement, HTMLCanvasElement, HTMLVideoElement> -> Sprite
        abstract fromFrame: frameId: string -> Sprite
        abstract fromImage: imageId: string * ?crossorigin: bool * ?scaleMode: float -> Sprite

    type [<AllowNullLiteral>] BatchBuffer =
        abstract vertices: ArrayBuffer with get, set
        abstract float32View: ResizeArray<float> with get, set
        abstract uint32View: ResizeArray<float> with get, set
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] BatchBufferStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> BatchBuffer

    type [<AllowNullLiteral>] SpriteRenderer =
        inherit ObjectRenderer
        abstract vertSize: float with get, set
        abstract vertByteSize: float with get, set
        abstract size: float with get, set
        abstract buffers: ResizeArray<BatchBuffer> with get, set
        abstract indices: ResizeArray<float> with get, set
        abstract shaders: ResizeArray<Shader> with get, set
        abstract currentIndex: float with get, set
        abstract tick: float with get, set
        abstract groups: ResizeArray<obj option> with get, set
        abstract sprites: ResizeArray<Sprite> with get, set
        abstract vertexBuffers: ResizeArray<float> with get, set
        abstract vaos: ResizeArray<GlCore.VertexArrayObject> with get, set
        abstract vaoMax: float with get, set
        abstract vertexCount: float with get, set
        abstract onContextChanged: (unit -> unit) with get, set
        abstract onPrerender: (unit -> unit) with get, set
        abstract render: sprite: Sprite -> unit
        abstract flush: unit -> unit
        abstract start: unit -> unit
        abstract stop: unit -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] SpriteRendererStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: PIXI.WebGLRenderer -> SpriteRenderer

    type [<AllowNullLiteral>] CanvasSpriteRenderer =
        inherit ObjectRenderer
        abstract render: sprite: Sprite -> unit
        abstract destroy: unit -> unit

    type [<AllowNullLiteral>] CanvasSpriteRendererStatic =
        [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> CanvasSpriteRenderer

    module CanvasTinter =

        type [<AllowNullLiteral>] IExports =
            abstract getTintedTexture: sprite: Sprite * color: float -> HTMLCanvasElement
            abstract tintWithMultiply: texture: Texture * color: float * canvas: HTMLCanvasElement -> unit
            abstract tintWithOverlay: texture: Texture * color: float * canvas: HTMLCanvasElement -> unit
            abstract tintWithPerPixel: texture: Texture * color: float * canvas: HTMLCanvasElement -> unit
            abstract roundColor: color: float -> float
            abstract cacheStepsPerColorChannel: float
            abstract convertTintToImage: bool
            abstract canUseMultiply: bool
            abstract tintMethod: float

    type [<AllowNullLiteral>] TextStyleOptions =
        abstract align: string option with get, set
        abstract breakWords: bool option with get, set
        abstract dropShadow: bool option with get, set
        abstract dropShadowAlpha: float option with get, set
        abstract dropShadowAngle: float option with get, set
        abstract dropShadowBlur: float option with get, set
        abstract dropShadowColor: U2<string, float> option with get, set
        abstract dropShadowDistance: float option with get, set
        abstract fill: U6<string, ResizeArray<string>, float, ResizeArray<float>, CanvasGradient, CanvasPattern> option with get, set
        abstract fillGradientType: float option with get, set
        abstract fillGradientStops: ResizeArray<float> option with get, set
        abstract fontFamily: U2<string, ResizeArray<string>> option with get, set
        abstract fontSize: U2<float, string> option with get, set
        abstract fontStyle: string option with get, set
        abstract fontVariant: string option with get, set
        abstract fontWeight: string option with get, set
        abstract letterSpacing: float option with get, set
        abstract lineHeight: float option with get, set
        abstract lineJoin: string option with get, set
        abstract miterLimit: float option with get, set
        abstract padding: float option with get, set
        abstract stroke: U2<string, float> option with get, set
        abstract strokeThickness: float option with get, set
        abstract textBaseline: string option with get, set
        abstract trim: bool option with get, set
        abstract whiteSpace: string option with get, set
        abstract wordWrap: bool option with get, set
        abstract wordWrapWidth: float option with get, set
        abstract leading: float option with get, set

    type [<AllowNullLiteral>] TextStyle =
        inherit TextStyleOptions
        abstract styleID: float with get, set
        abstract clone: unit -> TextStyle
        abstract reset: unit -> unit
        abstract _align: string with get, set
        abstract align: string with get, set
        abstract _breakWords: bool with get, set
        abstract breakWords: bool with get, set
        abstract _dropShadow: bool with get, set
        abstract dropShadow: bool with get, set
        abstract _dropShadowAlpha: float with get, set
        abstract dropShadowAlpha: float with get, set
        abstract _dropShadowAngle: float with get, set
        abstract dropShadowAngle: float with get, set
        abstract _dropShadowBlur: float with get, set
        abstract dropShadowBlur: float with get, set
        abstract _dropShadowColor: U2<string, float> with get, set
        abstract dropShadowColor: U2<string, float> with get, set
        abstract _dropShadowDistance: float with get, set
        abstract dropShadowDistance: float with get, set
        abstract _fill: U6<string, ResizeArray<string>, float, ResizeArray<float>, CanvasGradient, CanvasPattern> with get, set
        abstract fill: U6<string, ResizeArray<string>, float, ResizeArray<float>, CanvasGradient, CanvasPattern> with get, set
        abstract _fillGradientType: float with get, set
        abstract fillGradientType: float with get, set
        abstract _fillGradientStops: ResizeArray<float> with get, set
        abstract fillGradientStops: ResizeArray<float> with get, set
        abstract _fontFamily: U2<string, ResizeArray<string>> with get, set
        abstract fontFamily: U2<string, ResizeArray<string>> with get, set
        abstract _fontSize: U2<float, string> with get, set
        abstract fontSize: U2<float, string> with get, set
        abstract _fontStyle: string with get, set
        abstract fontStyle: string with get, set
        abstract _fontVariant: string with get, set
        abstract fontVariant: string with get, set
        abstract _fontWeight: string with get, set
        abstract fontWeight: string with get, set
        abstract _leading: float with get, set
        abstract leading: float with get, set
        abstract _letterSpacing: float with get, set
        abstract letterSpacing: float with get, set
        abstract _lineHeight: float with get, set
        abstract lineHeight: float with get, set
        abstract _lineJoin: string with get, set
        abstract lineJoin: string with get, set
        abstract _miterLimit: float with get, set
        abstract miterLimit: float with get, set
        abstract _padding: float with get, set
        abstract padding: float with get, set
        abstract _stroke: U2<string, float> with get, set
        abstract stroke: U2<string, float> with get, set
        abstract _strokeThickness: float with get, set
        abstract strokeThickness: float with get, set
        abstract _textBaseline: string with get, set
        abstract textBaseline: string with get, set
        abstract _trim: bool with get, set
        abstract trim: bool with get, set
        abstract _whiteSpace: string with get, set
        abstract whiteSpace: string with get, set
        abstract _wordWrap: bool with get, set
        abstract wordWrap: bool with get, set
        abstract _wordWrapWidth: float with get, set
        abstract wordWrapWidth: float with get, set
        abstract toFontString: unit -> string

    type [<AllowNullLiteral>] TextStyleStatic =
        [<Emit "new $0($1...)">] abstract Create: style: TextStyleOptions -> TextStyle

    type [<AllowNullLiteral>] TextMetrics =
        abstract METRICS_STRING: string with get, set
        abstract BASELINE_SYMBOL: string with get, set
        abstract BASELINE_MULTIPLIER: float with get, set
        abstract _canvas: HTMLCanvasElement with get, set
        abstract _context: CanvasRenderingContext2D with get, set
        abstract _fonts: FontMetrics with get, set
        abstract _newLines: ResizeArray<float> with get, set
        abstract _breakingSpaces: ResizeArray<float> with get, set
        abstract text: string with get, set
        abstract style: TextStyle with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract lines: ResizeArray<float> with get, set
        abstract lineWidths: ResizeArray<float> with get, set
        abstract lineHeight: float with get, set
        abstract maxLineWidth: float with get, set
        abstract fontProperties: obj option with get, set

    type [<AllowNullLiteral>] TextMetricsStatic =
        [<Emit "new $0($1...)">] abstract Create: text: string * style: TextStyle * width: float * height: float * lines: ResizeArray<float> * lineWidths: ResizeArray<float> * lineHeight: float * maxLineWidth: float * fontProperties: obj option -> TextMetrics
        abstract measureText: text: string * style: TextStyle * ?wordWrap: bool * ?canvas: HTMLCanvasElement -> TextMetrics
        abstract wordWrap: text: string * style: TextStyle * ?canvas: HTMLCanvasElement -> string
        abstract addLine: line: string * ?newLine: bool -> string
        abstract getFromCache: key: string * letterSpacing: float * cache: obj option * context: CanvasRenderingContext2D -> float
        abstract collapseSpaces: ?whiteSpace: string -> bool
        abstract collapseNewlines: ?whiteSpace: string -> bool
        abstract trimRight: ?text: string -> string
        abstract isNewline: ?char: string -> bool
        abstract isBreakingSpace: ?char: string -> bool
        abstract tokenize: ?text: string -> ResizeArray<string>
        abstract canBreakWords: ?token: string * ?breakWords: bool -> bool
        abstract canBreakChars: char: string * nextChar: string * token: string * index: float * ?breakWords: bool -> bool
        abstract measureFont: font: string -> FontMetrics
        abstract clearMetrics: font: string -> unit

    type [<AllowNullLiteral>] FontMetrics =
        abstract ascent: float with get, set
        abstract descent: float with get, set
        abstract fontSize: float with get, set

    type [<AllowNullLiteral>] Text =
        inherit Sprite
        abstract canvas: HTMLCanvasElement with get, set
        abstract context: CanvasRenderingContext2D with get, set
        abstract resolution: float with get, set
        abstract _text: string with get, set
        abstract _style: TextStyle with get, set
        abstract _styleListener: Function with get, set
        abstract _font: string with get, set
        abstract localStyleID: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract style: TextStyle with get, set
        abstract text: string with get, set
        abstract updateText: ?respectDirty: bool -> unit
        abstract drawLetterSpacing: text: string * x: float * y: float * ?isStroke: bool -> unit
        abstract updateTexture: unit -> unit
        abstract renderWebGL: renderer: WebGLRenderer -> unit
        abstract _renderCanvas: renderer: CanvasRenderer -> unit
        abstract getLocalBounds: ?rect: Rectangle -> Rectangle
        abstract _calculateBounds: unit -> unit
        abstract _onStyleChange: (unit -> unit) with get, set
        abstract _generateFillStyle: style: TextStyle * lines: ResizeArray<string> -> U3<string, float, CanvasGradient>
        abstract destroy: ?options: U2<DestroyOptions, bool> -> unit
        abstract dirty: bool with get, set

    type [<AllowNullLiteral>] TextStatic =
        [<Emit "new $0($1...)">] abstract Create: ?text: string * ?style: TextStyleOptions * ?canvas: HTMLCanvasElement -> Text

    type [<AllowNullLiteral>] BaseRenderTexture =
        inherit BaseTexture
        abstract height: float with get, set
        abstract width: float with get, set
        abstract realHeight: float with get, set
        abstract realWidth: float with get, set
        abstract resolution: float with get, set
        abstract scaleMode: float with get, set
        abstract hasLoaded: bool with get, set
        abstract _glRenderTargets: obj with get, set
        abstract _canvasRenderTarget: obj with get, set
        abstract valid: bool with get, set
        abstract resize: width: float * height: float -> unit
        abstract destroy: unit -> unit
        [<Emit "$0.on('update',$1,$2)">] abstract on_update: fn: (BaseRenderTexture -> unit) * ?context: obj option -> BaseRenderTexture
        [<Emit "$0.once('update',$1,$2)">] abstract once_update: fn: (BaseRenderTexture -> unit) * ?context: obj option -> BaseRenderTexture
        [<Emit "$0.removeListener('update',$1,$2)">] abstract removeListener_update: ?fn: (BaseRenderTexture -> unit) * ?context: obj option -> BaseRenderTexture
        [<Emit "$0.removeAllListeners('update')">] abstract removeAllListeners_update: unit -> BaseRenderTexture
        [<Emit "$0.off('update',$1,$2)">] abstract off_update: ?fn: (BaseRenderTexture -> unit) * ?context: obj option -> BaseRenderTexture
        [<Emit "$0.addListener('update',$1,$2)">] abstract addListener_update: fn: (BaseRenderTexture -> unit) * ?context: obj option -> BaseRenderTexture

    type [<AllowNullLiteral>] BaseRenderTextureStatic =
        [<Emit "new $0($1...)">] abstract Create: ?width: float * ?height: float * ?scaleMode: float * ?resolution: float -> BaseRenderTexture

    type [<AllowNullLiteral>] BaseTexture =
        inherit Utils.EventEmitter
        abstract uuid: float option with get, set
        abstract touched: float with get, set
        abstract resolution: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract realWidth: float with get, set
        abstract realHeight: float with get, set
        abstract scaleMode: float with get, set
        abstract hasLoaded: bool with get, set
        abstract isLoading: bool with get, set
        abstract wrapMode: float with get, set
        abstract source: U3<HTMLImageElement, HTMLCanvasElement, HTMLVideoElement> option with get, set
        abstract origSource: HTMLImageElement option with get, set
        abstract imageType: string option with get, set
        abstract sourceScale: float with get, set
        abstract premultipliedAlpha: bool with get, set
        abstract imageUrl: string option with get, set
        abstract isPowerOfTwo: bool with get, set
        abstract mipmap: bool with get, set
        abstract wrap: bool option with get, set
        abstract _glTextures: obj option with get, set
        abstract _enabled: float with get, set
        abstract _id: float option with get, set
        abstract _virtualBoundId: float with get, set
        abstract _destroyed: bool
        abstract textureCacheIds: ResizeArray<string> with get, set
        abstract update: unit -> unit
        abstract _updateDimensions: unit -> unit
        abstract _updateImageType: unit -> unit
        abstract _loadSvgSource: unit -> unit
        abstract _loadSvgSourceUsingDataUri: dataUri: string -> unit
        abstract _loadSvgSourceUsingXhr: unit -> unit
        abstract _loadSvgSourceUsingString: svgString: string -> unit
        abstract loadSource: source: U3<HTMLImageElement, HTMLCanvasElement, HTMLVideoElement> -> unit
        abstract _sourceLoaded: unit -> unit
        abstract destroy: unit -> unit
        abstract dispose: unit -> unit
        abstract updateSourceImage: newSrc: string -> unit
        abstract on: ``event``: U4<string, string, string, string> * fn: (BaseTexture -> unit) * ?context: obj option -> BaseTexture
        abstract once: ``event``: U4<string, string, string, string> * fn: (BaseTexture -> unit) * ?context: obj option -> BaseTexture
        abstract removeListener: ``event``: U4<string, string, string, string> * ?fn: (BaseTexture -> unit) * ?context: obj option -> BaseTexture
        abstract removeAllListeners: ?``event``: U4<string, string, string, string> -> BaseTexture
        abstract off: ``event``: U4<string, string, string, string> * ?fn: (BaseTexture -> unit) * ?context: obj option -> BaseTexture
        abstract addListener: ``event``: U4<string, string, string, string> * fn: (BaseTexture -> unit) * ?context: obj option -> BaseTexture

    type [<AllowNullLiteral>] BaseTextureStatic =
        abstract from: source: U3<string, HTMLImageElement, HTMLCanvasElement> * ?scaleMode: float * ?sourceScale: float -> BaseTexture
        [<Emit "new $0($1...)">] abstract Create: ?source: U3<HTMLImageElement, HTMLCanvasElement, HTMLVideoElement> * ?scaleMode: float * ?resolution: float -> BaseTexture
        abstract fromImage: imageUrl: string * ?crossorigin: bool * ?scaleMode: float * ?sourceScale: float -> BaseTexture
        abstract fromCanvas: canvas: HTMLCanvasElement * ?scaleMode: float * ?origin: string -> BaseTexture
        abstract addToCache: baseTexture: BaseTexture * id: string -> unit
        abstract removeFromCache: baseTexture: U2<string, BaseTexture> -> BaseTexture

    type [<AllowNullLiteral>] RenderTexture =
        inherit Texture
        abstract legacyRenderer: obj option with get, set
        abstract valid: bool with get, set
        abstract resize: width: float * height: float * ?doNotResizeBaseTexture: bool -> unit

    type [<AllowNullLiteral>] RenderTextureStatic =
        [<Emit "new $0($1...)">] abstract Create: baseRenderTexture: BaseRenderTexture * ?frame: Rectangle -> RenderTexture
        abstract create: ?width: float * ?height: float * ?scaleMode: float * ?resolution: float -> RenderTexture

    type [<AllowNullLiteral>] Texture =
        inherit Utils.EventEmitter
        abstract noFrame: bool with get, set
        abstract baseTexture: BaseTexture with get, set
        abstract _frame: Rectangle with get, set
        abstract trim: Rectangle option with get, set
        abstract valid: bool with get, set
        abstract requiresUpdate: bool with get, set
        abstract _uvs: TextureUvs with get, set
        abstract orig: Rectangle with get, set
        abstract _updateID: float with get, set
        abstract transform: TextureMatrix with get, set
        abstract textureCacheIds: ResizeArray<string> with get, set
        abstract update: unit -> unit
        abstract onBaseTextureLoaded: baseTexture: BaseTexture -> unit
        abstract onBaseTextureUpdated: baseTexture: BaseTexture -> unit
        abstract destroy: ?destroyBase: bool -> unit
        abstract clone: unit -> Texture
        abstract _updateUvs: unit -> unit
        abstract frame: Rectangle with get, set
        abstract _rotate: U2<bool, obj> with get, set
        abstract rotate: float with get, set
        abstract width: float with get, set
        abstract height: float with get, set
        abstract EMPTY: Texture with get, set
        abstract WHITE: Texture with get, set
        [<Emit "$0.on('update',$1,$2)">] abstract on_update: fn: (Texture -> unit) * ?context: obj option -> Texture
        [<Emit "$0.once('update',$1,$2)">] abstract once_update: fn: (Texture -> unit) * ?context: obj option -> Texture
        [<Emit "$0.removeListener('update',$1,$2)">] abstract removeListener_update: ?fn: (Texture -> unit) * ?context: obj option -> Texture
        [<Emit "$0.removeAllListeners('update')">] abstract removeAllListeners_update: unit -> Texture
        [<Emit "$0.off('update',$1,$2)">] abstract off_update: ?fn: (Texture -> unit) * ?context: obj option -> Texture
        [<Emit "$0.addListener('update',$1,$2)">] abstract addListener_update: fn: (Texture -> unit) * ?context: obj option -> Texture

    type [<AllowNullLiteral>] TextureStatic =
        [<Emit "new $0($1...)">] abstract Create: baseTexture: BaseTexture * ?frame: Rectangle * ?orig: Rectangle * ?trim: Rectangle * ?rotate: float -> Texture
        abstract fromImage: imageUrl: string * ?crossOrigin: bool * ?scaleMode: float * ?sourceScale: float -> Texture
        abstract fromFrame: frameId: string -> Texture
        abstract fromCanvas: canvas: HTMLCanvasElement * ?scaleMode: float * ?origin: string -> Texture
        abstract fromVideo: video: U2<HTMLVideoElement, string> * ?scaleMode: float -> Texture
        abstract fromVideoUrl: videoUrl: string * ?scaleMode: float -> Texture
        abstract from: source: U6<float, string, HTMLImageElement, HTMLCanvasElement, HTMLVideoElement, BaseTexture> -> Texture
        abstract fromLoader: source: U2<HTMLImageElement, HTMLCanvasElement> * imageUrl: string * ?name: string -> Texture
        abstract addToCache: texture: Texture * id: string -> unit
        abstract removeFromCache: texture: U2<string, Texture> -> Texture
        abstract addTextureToCache: texture: Texture * id: string -> unit
        abstract removeTextureFromCache: id: string -> Texture

    type [<AllowNullLiteral>] TextureMatrix =
        abstract _texture: Texture with get, set
        abstract mapCoord: Matrix with get, set
        abstract uClampFrame: Float32Array with get, set
        abstract uClampOffset: Float32Array with get, set
        abstract _lastTextureID: float with get, set
        abstract clampOffset: float with get, set
        abstract clampMargin: float with get, set
        abstract texture: Texture with get, set
        abstract update: ?forceUpdate: bool -> bool
        abstract multiplyUvs: uvs: Float32Array * ?out: Float32Array -> Float32Array

    type [<AllowNullLiteral>] TextureMatrixStatic =
        [<Emit "new $0($1...)">] abstract Create: texture: Texture * ?clampMargin: float -> TextureMatrix

    type [<AllowNullLiteral>] TextureUvs =
        abstract x0: float with get, set
        abstract y0: float with get, set
        abstract x1: float with get, set
        abstract y1: float with get, set
        abstract x2: float with get, set
        abstract y2: float with get, set
        abstract x3: float with get, set
        abstract y3: float with get, set
        abstract uvsUint32: Uint32Array with get, set
        abstract set: frame: Rectangle * baseFrame: Rectangle * rotate: float -> unit

    type [<AllowNullLiteral>] TextureUvsStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> TextureUvs

    type [<AllowNullLiteral>] Spritesheet =
        abstract BATCH_SIZE: float with get, set
        abstract baseTexture: BaseTexture with get, set
        abstract textures: obj with get, set
        abstract data: obj option with get, set
        abstract resolution: float with get, set
        abstract _frames: obj option with get, set
        abstract _frameKeys: string with get, set
        abstract _batchIndex: float with get, set
        abstract _callback: (Spritesheet -> obj -> unit) with get, set
        abstract _updateResolution: resolutionFilename: string -> float
        abstract parse: callback: (Spritesheet -> obj -> unit) -> unit
        abstract _processFrames: initialFrameIndex: float -> unit
        abstract _parseComplete: unit -> unit
        abstract _nextBatch: unit -> unit
        abstract destroy: ?destroyBase: bool -> unit

    type [<AllowNullLiteral>] SpritesheetStatic =
        [<Emit "new $0($1...)">] abstract Create: baseTexture: BaseTexture * data: obj option * ?resolutionFilename: string -> Spritesheet

    type [<AllowNullLiteral>] VideoBaseTexture =
        inherit BaseTexture
        abstract autoUpdate: bool with get, set
        abstract autoPlay: bool with get, set
        abstract _isAutoUpdating: bool with get, set
        abstract update: unit -> unit
        abstract _onCanPlay: unit -> unit
        abstract _onPlayStart: unit -> unit
        abstract _onPlayStop: unit -> unit
        abstract destroy: unit -> unit
        abstract _isSourcePlaying: unit -> bool
        abstract _isSourceReady: unit -> bool
        abstract source: HTMLVideoElement with get, set
        abstract loadSource: source: HTMLVideoElement -> unit

    type [<AllowNullLiteral>] VideoBaseTextureStatic =
        [<Emit "new $0($1...)">] abstract Create: source: HTMLVideoElement * ?scaleMode: float -> VideoBaseTexture
        abstract fromVideo: video: HTMLVideoElement * ?scaleMode: float -> VideoBaseTexture
        abstract fromUrl: videoSrc: U4<string, obj option, ResizeArray<string>, ResizeArray<obj option>> * ?crossOrigin: bool -> VideoBaseTexture
        abstract fromUrls: videoSrc: U4<string, obj option, ResizeArray<string>, ResizeArray<obj option>> -> VideoBaseTexture

    module Ticker =

        type [<AllowNullLiteral>] IExports =
            abstract shared: Ticker
            abstract TickerListener: TickerListenerStatic
            abstract Ticker: TickerStatic

        type [<AllowNullLiteral>] TickerListener =
            abstract fn: (float -> unit) with get, set
            abstract context: obj option with get, set
            abstract priority: float with get, set
            abstract once: bool with get, set
            abstract next: TickerListener with get, set
            abstract previous: TickerListener with get, set
            abstract _destroyed: bool with get, set
            abstract ``match``: fn: (float -> unit) * ?context: obj option -> bool
            abstract emit: deltaTime: float -> TickerListener
            abstract connect: previous: TickerListener -> unit
            abstract destroy: ?hard: bool -> unit

        type [<AllowNullLiteral>] TickerListenerStatic =
            [<Emit "new $0($1...)">] abstract Create: fn: (float -> unit) * ?context: obj option * ?priority: float * ?once: bool -> TickerListener

        type [<AllowNullLiteral>] Ticker =
            abstract _tick: (float -> unit) with get, set
            abstract _head: TickerListener with get, set
            abstract _requestId: float option with get, set
            abstract _maxElapsedMS: float with get, set
            abstract autoStart: bool with get, set
            abstract deltaTime: float with get, set
            abstract elapsedMS: float with get, set
            abstract lastTime: float with get, set
            abstract speed: float with get, set
            abstract started: bool with get, set
            abstract _requestIfNeeded: unit -> unit
            abstract _cancelIfNeeded: unit -> unit
            abstract _startIfPossible: unit -> unit
            abstract add: fn: (float -> unit) * ?context: obj option * ?priority: float -> Ticker
            abstract addOnce: fn: (float -> unit) * ?context: obj option * ?priority: float -> Ticker
            abstract remove: fn: Function * ?context: obj option * ?priority: float -> Ticker
            abstract _addListener: listener: TickerListener -> Ticker
            abstract FPS: float
            abstract minFPS: float with get, set
            abstract start: unit -> unit
            abstract stop: unit -> unit
            abstract destroy: unit -> unit
            abstract update: ?currentTime: float -> unit

        type [<AllowNullLiteral>] TickerStatic =
            [<Emit "new $0($1...)">] abstract Create: unit -> Ticker

    type [<AllowNullLiteral>] Shader =
        inherit GlCore.GLShader

    type [<AllowNullLiteral>] ShaderStatic =
        [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * vertexSrc: U2<string, ResizeArray<string>> * fragmentSrc: U2<string, ResizeArray<string>> * ?attributeLocations: ShaderStaticAttributeLocations * ?precision: string -> Shader

    type [<AllowNullLiteral>] ShaderStaticAttributeLocations =
        [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> float with get, set

    module Extract =

        type [<AllowNullLiteral>] IExports =
            abstract CanvasExtract: CanvasExtractStatic
            abstract WebGLExtract: WebGLExtractStatic

        type [<AllowNullLiteral>] CanvasExtract =
            abstract renderer: CanvasRenderer with get, set
            abstract image: ?target: U2<DisplayObject, RenderTexture> -> HTMLImageElement
            abstract base64: ?target: U2<DisplayObject, RenderTexture> -> string
            abstract canvas: ?target: U2<DisplayObject, RenderTexture> -> HTMLCanvasElement
            abstract pixels: ?renderTexture: U2<DisplayObject, RenderTexture> -> Uint8ClampedArray
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] CanvasExtractStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: CanvasRenderer -> CanvasExtract

        type [<AllowNullLiteral>] WebGLExtract =
            abstract renderer: WebGLRenderer with get, set
            abstract image: ?target: U2<DisplayObject, RenderTexture> -> HTMLImageElement
            abstract base64: ?target: U2<DisplayObject, RenderTexture> -> string
            abstract canvas: ?target: U2<DisplayObject, RenderTexture> -> HTMLCanvasElement
            abstract pixels: ?renderTexture: U2<DisplayObject, RenderTexture> -> Uint8Array
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] WebGLExtractStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> WebGLExtract

    module Extras =

        type [<AllowNullLiteral>] IExports =
            abstract BitmapText: BitmapTextStatic
            abstract AnimatedSprite: AnimatedSpriteStatic
            abstract TextureMatrix: TextureMatrixStatic
            abstract TilingSprite: TilingSpriteStatic
            abstract TilingSpriteRenderer: TilingSpriteRendererStatic

        type [<AllowNullLiteral>] BitmapTextStyle =
            abstract font: U2<string, obj> option with get, set
            abstract align: string option with get, set
            abstract tint: float option with get, set

        type [<AllowNullLiteral>] BitmapText =
            inherit Container
            abstract letterSpacing: float with get, set
            abstract _letterSpacing: float with get, set
            abstract _textWidth: float with get, set
            abstract _textHeight: float with get, set
            abstract textWidth: float with get, set
            abstract textHeight: float with get, set
            abstract _glyphs: ResizeArray<Sprite> with get, set
            abstract _font: U2<string, obj> with get, set
            abstract font: U2<string, obj> with get, set
            abstract _text: string with get, set
            abstract _maxWidth: float with get, set
            abstract maxWidth: float with get, set
            abstract _maxLineHeight: float with get, set
            abstract maxLineHeight: float with get, set
            abstract _anchor: ObservablePoint with get, set
            abstract dirty: bool with get, set
            abstract tint: float with get, set
            abstract align: string with get, set
            abstract text: string with get, set
            abstract anchor: U2<PIXI.Point, float> with get, set
            abstract updateText: unit -> unit
            abstract updateTransform: unit -> unit
            abstract getLocalBounds: unit -> Rectangle
            abstract validate: unit -> unit
            abstract fonts: obj option with get, set

        type [<AllowNullLiteral>] BitmapTextStatic =
            abstract registerFont: xml: XMLDocument * textures: U3<Texture, ResizeArray<Texture>, obj> -> obj option
            [<Emit "new $0($1...)">] abstract Create: text: string * ?style: BitmapTextStyle -> BitmapText

        type [<AllowNullLiteral>] AnimatedSpriteTextureTimeObject =
            abstract texture: Texture with get, set
            abstract time: float option with get, set

        type [<AllowNullLiteral>] AnimatedSprite =
            inherit Sprite
            abstract _autoUpdate: bool with get, set
            abstract _textures: ResizeArray<Texture> with get, set
            abstract _durations: ResizeArray<float> with get, set
            abstract textures: U2<ResizeArray<Texture>, ResizeArray<AnimatedSpriteTextureTimeObject>> with get, set
            abstract animationSpeed: float with get, set
            abstract loop: bool with get, set
            abstract onComplete: (unit -> unit) with get, set
            abstract onFrameChange: (float -> unit) with get, set
            abstract onLoop: (unit -> unit) with get, set
            abstract _currentTime: float with get, set
            abstract playing: bool with get, set
            abstract totalFrames: float with get, set
            abstract currentFrame: float with get, set
            abstract stop: unit -> unit
            abstract play: unit -> unit
            abstract gotoAndStop: frameNumber: float -> unit
            abstract gotoAndPlay: frameNumber: float -> unit
            abstract update: deltaTime: float -> unit
            abstract destroy: ?options: U2<DestroyOptions, bool> -> unit

        type [<AllowNullLiteral>] AnimatedSpriteStatic =
            [<Emit "new $0($1...)">] abstract Create: textures: U2<ResizeArray<Texture>, ResizeArray<AnimatedSpriteTextureTimeObject>> * ?autoUpdate: bool -> AnimatedSprite
            abstract fromFrames: frame: ResizeArray<string> -> AnimatedSprite
            abstract fromImages: images: ResizeArray<string> -> AnimatedSprite

        type [<AllowNullLiteral>] TextureMatrix =
            abstract _texture: Texture with get, set
            abstract mapCoord: Matrix with get, set
            abstract uClampFrame: Float32Array with get, set
            abstract uClampOffset: Float32Array with get, set
            abstract _lastTextureID: float with get, set
            abstract clampOffset: float with get, set
            abstract clampMargin: float with get, set
            abstract texture: Texture with get, set
            abstract update: ?forceUpdate: bool -> bool
            abstract multiplyUvs: uvs: Float32Array * ?out: Float32Array -> Float32Array

        type [<AllowNullLiteral>] TextureMatrixStatic =
            [<Emit "new $0($1...)">] abstract Create: texture: Texture * ?clampMargin: float -> TextureMatrix

        type [<AllowNullLiteral>] TilingSprite =
            inherit Sprite
            abstract tileTransform: Transform with get, set
            abstract _width: float with get, set
            abstract _height: float with get, set
            abstract _canvasPattern: CanvasPattern with get, set
            abstract uvTransform: TextureMatrix with get, set
            abstract uvRespectAnchor: bool with get, set
            abstract clampMargin: float with get, set
            abstract tileScale: U2<Point, ObservablePoint> with get, set
            abstract tilePosition: U2<Point, ObservablePoint> with get, set
            abstract multiplyUvs: uvs: Float32Array * out: Float32Array -> Float32Array
            abstract _onTextureUpdate: unit -> unit
            abstract _renderWebGL: renderer: WebGLRenderer -> unit
            abstract _renderCanvas: renderer: CanvasRenderer -> unit
            abstract _calculateBounds: unit -> unit
            abstract getLocalBounds: ?rect: Rectangle -> Rectangle
            abstract containsPoint: point: Point -> bool
            abstract destroy: ?options: U2<DestroyOptions, bool> -> unit
            abstract width: float with get, set
            abstract height: float with get, set

        type [<AllowNullLiteral>] TilingSpriteStatic =
            [<Emit "new $0($1...)">] abstract Create: texture: Texture * ?width: float * ?height: float -> TilingSprite
            abstract from: source: U5<float, string, BaseTexture, HTMLCanvasElement, HTMLVideoElement> * ?width: float * ?height: float -> TilingSprite
            abstract fromFrame: frameId: string * ?width: float * ?height: float -> TilingSprite
            abstract fromImage: imageId: string * ?crossorigin: bool * ?scaleMode: float -> Sprite
            abstract fromImage: imageId: string * ?width: float * ?height: float * ?crossorigin: bool * ?scaleMode: float -> TilingSprite

        type [<AllowNullLiteral>] TilingSpriteRenderer =
            inherit ObjectRenderer
            abstract render: ts: TilingSprite -> unit

        type [<AllowNullLiteral>] TilingSpriteRendererStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> TilingSpriteRenderer

        type MovieClip =
            Extras.AnimatedSprite

        type TextureTranform =
            TextureMatrix

    module Filters =

        type [<AllowNullLiteral>] IExports =
            abstract FXAAFilter: FXAAFilterStatic
            abstract BlurFilter: BlurFilterStatic
            abstract BlurXFilter: BlurXFilterStatic
            abstract BlurYFilter: BlurYFilterStatic
            abstract ColorMatrixFilter: ColorMatrixFilterStatic
            abstract DisplacementFilter: DisplacementFilterStatic
            abstract AlphaFilter: AlphaFilterStatic
            abstract NoiseFilter: NoiseFilterStatic

        type [<AllowNullLiteral>] FXAAFilter =
            inherit Filter<obj>

        type [<AllowNullLiteral>] FXAAFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: unit -> FXAAFilter

        type [<AllowNullLiteral>] BlurFilter =
            inherit Filter<obj>
            abstract blurXFilter: BlurXFilter with get, set
            abstract blurYFilter: BlurYFilter with get, set
            abstract resolution: float with get, set
            abstract padding: float with get, set
            abstract passes: float with get, set
            abstract blur: float with get, set
            abstract blurX: float with get, set
            abstract blurY: float with get, set
            abstract quality: float with get, set
            abstract blendMode: float with get, set

        type [<AllowNullLiteral>] BlurFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: ?strength: float * ?quality: float * ?resolution: float * ?kernelSize: float -> BlurFilter

        type [<AllowNullLiteral>] BlurXFilterUniforms =
            abstract strength: float with get, set

        type [<AllowNullLiteral>] BlurXFilter =
            inherit Filter<BlurXFilterUniforms>
            abstract _quality: float with get, set
            abstract quality: float with get, set
            abstract passes: float with get, set
            abstract resolution: float with get, set
            abstract strength: float with get, set
            abstract firstRun: bool with get, set
            abstract blur: float with get, set

        type [<AllowNullLiteral>] BlurXFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: ?strength: float * ?quality: float * ?resolution: float * ?kernelSize: float -> BlurXFilter

        type [<AllowNullLiteral>] BlurYFilterUniforms =
            abstract strength: float with get, set

        type [<AllowNullLiteral>] BlurYFilter =
            inherit Filter<BlurYFilterUniforms>
            abstract _quality: float with get, set
            abstract quality: float with get, set
            abstract passes: float with get, set
            abstract resolution: float with get, set
            abstract strength: float with get, set
            abstract firstRun: bool with get, set
            abstract blur: float with get, set

        type [<AllowNullLiteral>] BlurYFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: ?strength: float * ?quality: float * ?resolution: float * ?kernelSize: float -> BlurYFilter

        type [<AllowNullLiteral>] ColorMatrixFilterUniforms =
            abstract m: Matrix with get, set
            abstract uAlpha: float with get, set

        type [<AllowNullLiteral>] ColorMatrixFilter =
            inherit Filter<ColorMatrixFilterUniforms>
            abstract _loadMatrix: matrix: ResizeArray<float> * ?multiply: bool -> unit
            abstract _multiply: out: ResizeArray<float> * a: ResizeArray<float> * b: ResizeArray<float> -> unit
            abstract _colorMatrix: matrix: ResizeArray<float> -> unit
            abstract matrix: ResizeArray<float> with get, set
            abstract alpha: float with get, set
            abstract brightness: b: float * ?multiply: bool -> unit
            abstract greyscale: scale: float * ?multiply: bool -> unit
            abstract blackAndWhite: ?multiply: bool -> unit
            abstract hue: rotation: float * ?multiply: bool -> unit
            abstract contrast: amount: float * ?multiply: bool -> unit
            abstract saturate: amount: float * ?multiply: bool -> unit
            abstract desaturate: ?multiply: bool -> unit
            abstract negative: ?multiply: bool -> unit
            abstract sepia: ?multiply: bool -> unit
            abstract technicolor: ?multiply: bool -> unit
            abstract polaroid: ?multiply: bool -> unit
            abstract toBGR: ?multiply: bool -> unit
            abstract kodachrome: ?multiply: bool -> unit
            abstract browni: ?multiply: bool -> unit
            abstract vintage: ?multiply: bool -> unit
            abstract colorTone: desaturation: float * toned: float * lightColor: string * darkColor: string * ?multiply: bool -> unit
            abstract night: intensity: float * ?multiply: bool -> unit
            abstract predator: amount: float * ?multiply: bool -> unit
            abstract lsd: ?multiply: bool -> unit
            abstract reset: unit -> unit

        type [<AllowNullLiteral>] ColorMatrixFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: unit -> ColorMatrixFilter

        type [<AllowNullLiteral>] DisplacementFilterUniforms =
            abstract mapSampler: Texture with get, set
            abstract filterMatrix: Matrix with get, set
            abstract scale: Point with get, set

        type [<AllowNullLiteral>] DisplacementFilter =
            inherit Filter<DisplacementFilterUniforms>
            abstract scale: Point with get, set
            abstract map: Texture with get, set

        type [<AllowNullLiteral>] DisplacementFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: sprite: Sprite * ?scale: float -> DisplacementFilter

        type [<AllowNullLiteral>] AlphaFilter =
            inherit Filter<obj>
            abstract alpha: float with get, set
            abstract glShaderKey: float with get, set

        type [<AllowNullLiteral>] AlphaFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: ?alpha: float -> AlphaFilter

        type [<AllowNullLiteral>] NoiseFilterUniforms =
            abstract uNoise: float with get, set
            abstract uSeed: float with get, set

        type [<AllowNullLiteral>] NoiseFilter =
            inherit Filter<NoiseFilterUniforms>
            abstract noise: float with get, set
            abstract seed: float with get, set

        type [<AllowNullLiteral>] NoiseFilterStatic =
            [<Emit "new $0($1...)">] abstract Create: ?noise: float * ?seed: float -> NoiseFilter

        type VoidFilter =
            Filters.AlphaFilter

    module Interaction =

        type [<AllowNullLiteral>] IExports =
            abstract InteractionData: InteractionDataStatic
            abstract InteractionManager: InteractionManagerStatic

        type [<AllowNullLiteral>] InteractiveTarget =
            abstract interactive: bool with get, set
            abstract interactiveChildren: bool with get, set
            abstract hitArea: U6<PIXI.Rectangle, PIXI.Circle, PIXI.Ellipse, PIXI.Polygon, PIXI.RoundedRectangle, PIXI.HitArea> with get, set
            abstract buttonMode: bool with get, set
            abstract cursor: string with get, set
            abstract trackedPointers: unit -> obj
            abstract defaultCursor: string with get, set

        type [<AllowNullLiteral>] InteractionTrackingData =
            abstract pointerId: float
            abstract flags: float with get, set
            abstract none: float with get, set
            abstract over: bool with get, set
            abstract rightDown: bool with get, set
            abstract leftDown: bool with get, set

        type [<AllowNullLiteral>] InteractionEvent =
            abstract stopped: bool with get, set
            abstract target: DisplayObject with get, set
            abstract currentTarget: DisplayObject with get, set
            abstract ``type``: string with get, set
            abstract data: InteractionData with get, set
            abstract stopPropagation: unit -> unit
            abstract reset: unit -> unit

        type [<AllowNullLiteral>] InteractionData =
            abstract ``global``: Point with get, set
            abstract target: DisplayObject with get, set
            abstract originalEvent: U3<MouseEvent, TouchEvent, PointerEvent> with get, set
            abstract identifier: float with get, set
            abstract isPrimary: bool with get, set
            abstract button: float with get, set
            abstract buttons: float with get, set
            abstract width: float with get, set
            abstract height: float with get, set
            abstract tiltX: float with get, set
            abstract tiltY: float with get, set
            abstract pointerType: string with get, set
            abstract pressure: float with get, set
            abstract rotationAngle: float with get, set
            abstract twist: float with get, set
            abstract tangentialPressure: float with get, set
            abstract pointerID: float
            abstract copyEvent: ``event``: U3<Touch, MouseEvent, PointerEvent> -> unit
            abstract reset: unit -> unit
            abstract getLocalPosition: displayObject: DisplayObject * ?point: Point * ?globalPos: Point -> Point

        type [<AllowNullLiteral>] InteractionDataStatic =
            [<Emit "new $0($1...)">] abstract Create: unit -> InteractionData

        type [<StringEnum>] [<RequireQualifiedAccess>] InteractionPointerEvents =
            | Pointerdown
            | Pointercancel
            | Pointerup
            | Pointertap
            | Pointerupoutside
            | Pointermove
            | Pointerover
            | Pointerout

        type [<StringEnum>] [<RequireQualifiedAccess>] InteractionTouchEvents =
            | Touchstart
            | Touchcancel
            | Touchend
            | Touchendoutside
            | Touchmove
            | Tap

        type [<StringEnum>] [<RequireQualifiedAccess>] InteractionMouseEvents =
            | Rightdown
            | Mousedown
            | Rightup
            | Mouseup
            | Rightclick
            | Click
            | Rightupoutside
            | Mouseupoutside
            | Mousemove
            | Mouseover
            | Mouseout

        type [<StringEnum>] [<RequireQualifiedAccess>] InteractionPixiEvents =
            | Added
            | Removed

        type InteractionEventTypes =
            U4<InteractionPointerEvents, InteractionTouchEvents, InteractionMouseEvents, InteractionPixiEvents>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module InteractionEventTypes =
            let ofInteractionPointerEvents v: InteractionEventTypes = v |> U4.Case1
            let isInteractionPointerEvents (v: InteractionEventTypes) = match v with U4.Case1 _ -> true | _ -> false
            let asInteractionPointerEvents (v: InteractionEventTypes) = match v with U4.Case1 o -> Some o | _ -> None
            let ofInteractionTouchEvents v: InteractionEventTypes = v |> U4.Case2
            let isInteractionTouchEvents (v: InteractionEventTypes) = match v with U4.Case2 _ -> true | _ -> false
            let asInteractionTouchEvents (v: InteractionEventTypes) = match v with U4.Case2 o -> Some o | _ -> None
            let ofInteractionMouseEvents v: InteractionEventTypes = v |> U4.Case3
            let isInteractionMouseEvents (v: InteractionEventTypes) = match v with U4.Case3 _ -> true | _ -> false
            let asInteractionMouseEvents (v: InteractionEventTypes) = match v with U4.Case3 o -> Some o | _ -> None
            let ofInteractionPixiEvents v: InteractionEventTypes = v |> U4.Case4
            let isInteractionPixiEvents (v: InteractionEventTypes) = match v with U4.Case4 _ -> true | _ -> false
            let asInteractionPixiEvents (v: InteractionEventTypes) = match v with U4.Case4 o -> Some o | _ -> None

        type [<AllowNullLiteral>] InteractionManagerOptions =
            abstract autoPreventDefault: bool option with get, set
            abstract interactionFrequency: float option with get, set

        type [<AllowNullLiteral>] InteractionManager =
            inherit Utils.EventEmitter
            abstract renderer: SystemRenderer with get, set
            abstract autoPreventDefault: bool with get, set
            abstract interactionFrequency: float with get, set
            abstract mouse: InteractionData with get, set
            abstract activeInteractionData: obj with get, set
            abstract interactionDataPool: ResizeArray<InteractionData> with get, set
            abstract eventData: InteractionEvent with get, set
            abstract interactionDOMElement: HTMLElement with get, set
            abstract moveWhenInside: bool with get, set
            abstract eventsAdded: bool with get, set
            abstract mouseOverRenderer: bool with get, set
            abstract supportsTouchEvents: bool
            abstract supportsPointerEvents: bool
            abstract onPointerUp: (PointerEvent -> unit) with get, set
            abstract processPointerUp: (InteractionEvent -> U3<Container, PIXI.Sprite, PIXI.Extras.TilingSprite> -> bool -> unit) with get, set
            abstract onPointerCancel: (PointerEvent -> unit) with get, set
            abstract processPointerCancel: (InteractionEvent -> U3<PIXI.Container, PIXI.Sprite, PIXI.Extras.TilingSprite> -> unit) with get, set
            abstract onPointerDown: (PointerEvent -> unit) with get, set
            abstract processPointerDown: (InteractionEvent -> U3<PIXI.Container, PIXI.Sprite, PIXI.Extras.TilingSprite> -> bool -> unit) with get, set
            abstract onPointerMove: (PointerEvent -> unit) with get, set
            abstract processPointerMove: (InteractionEvent -> U3<PIXI.Container, PIXI.Sprite, PIXI.Extras.TilingSprite> -> bool -> unit) with get, set
            abstract onPointerOut: (PointerEvent -> unit) with get, set
            abstract processPointerOverOut: (InteractionEvent -> U3<PIXI.Container, PIXI.Sprite, PIXI.Extras.TilingSprite> -> bool -> unit) with get, set
            abstract onPointerOver: (PointerEvent -> unit) with get, set
            abstract cursorStyles: obj with get, set
            abstract currentCursorMode: string with get, set
            abstract cursor: string with get, set
            abstract _tempPoint: Point with get, set
            abstract resolution: float with get, set
            abstract hitTest: globalPoint: Point * ?root: Container -> DisplayObject
            abstract setTargetElement: element: HTMLCanvasElement * ?resolution: float -> unit
            abstract addEvents: unit -> unit
            abstract removeEvents: unit -> unit
            abstract update: ?deltaTime: float -> unit
            abstract setCursorMode: mode: string -> unit
            abstract dispatchEvent: displayObject: U3<Container, Sprite, Extras.TilingSprite> * eventString: string * eventData: obj option -> unit
            abstract mapPositionToPoint: point: Point * x: float * y: float -> unit
            abstract processInteractive: interactionEvent: InteractionEvent * displayObject: U3<PIXI.Container, PIXI.Sprite, PIXI.Extras.TilingSprite> * ?func: Function * ?hitTest: bool * ?interactive: bool -> bool
            abstract onPointerComplete: originalEvent: PointerEvent * cancelled: bool * func: Function -> unit
            abstract getInteractionDataForPointerId: pointerId: float -> InteractionData
            abstract releaseInteractionDataForPointerId: ``event``: PointerEvent -> unit
            abstract configureInteractionEventForDOMEvent: interactionEvent: InteractionEvent * pointerEvent: PointerEvent * interactionData: InteractionData -> InteractionEvent
            abstract normalizeToPointerData: ``event``: U3<TouchEvent, MouseEvent, PointerEvent> -> ResizeArray<PointerEvent>
            abstract destroy: unit -> unit
            abstract defaultCursorStyle: string with get, set
            abstract currentCursorStyle: string with get, set

        type [<AllowNullLiteral>] InteractionManagerStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: U3<CanvasRenderer, WebGLRenderer, SystemRenderer> * ?options: InteractionManagerOptions -> InteractionManager

    type [<AllowNullLiteral>] MiniSignalBinding =
        abstract _fn: Function with get, set
        abstract _once: bool with get, set
        abstract _thisArg: obj option with get, set
        abstract _next: MiniSignalBinding with get, set
        abstract _prev: MiniSignalBinding with get, set
        abstract _owner: MiniSignal with get, set
        abstract detach: unit -> bool

    type [<AllowNullLiteral>] MiniSignalBindingStatic =
        [<Emit "new $0($1...)">] abstract Create: fn: Function * ?once: bool * ?thisArg: obj option -> MiniSignalBinding

    type [<AllowNullLiteral>] MiniSignal =
        abstract _head: MiniSignalBinding with get, set
        abstract _tail: MiniSignalBinding with get, set
        abstract handlers: ?exists: bool -> U2<ResizeArray<MiniSignalBinding>, bool>
        abstract handlers: ?exists: obj -> bool
        abstract has: node: MiniSignalBinding -> bool
        abstract dispatch: unit -> bool
        abstract add: fn: Function * ?thisArg: obj option -> obj option
        abstract once: fn: Function * ?thisArg: obj option -> obj option
        abstract detach: node: MiniSignalBinding -> MiniSignal
        abstract detachAll: unit -> MiniSignal

    type [<AllowNullLiteral>] MiniSignalStatic =
        [<Emit "new $0($1...)">] abstract Create: unit -> MiniSignal

    module Loaders =

        type [<AllowNullLiteral>] IExports =
            abstract Loader: LoaderStatic
            abstract Resource: ResourceStatic
            abstract shared: Loader

        type [<AllowNullLiteral>] LoaderOptions =
            abstract crossOrigin: U2<bool, string> option with get, set
            abstract loadType: float option with get, set
            abstract xhrType: string option with get, set
            abstract metaData: obj option with get, set

        type [<AllowNullLiteral>] ResourceDictionary =
            [<Emit "$0[$1]{{=$2}}">] abstract Item: index: string -> PIXI.Loaders.Resource with get, set

        type [<AllowNullLiteral>] Loader =
            inherit Utils.EventEmitter
            abstract Resource: obj option with get, set
            abstract async: obj option with get, set
            abstract base64: obj option with get, set
            abstract baseUrl: string with get, set
            abstract progress: float with get, set
            abstract loading: bool with get, set
            abstract defaultQueryString: string with get, set
            abstract _beforeMiddleware: ResizeArray<Function> with get, set
            abstract _afterMiddleware: ResizeArray<Function> with get, set
            abstract _resourcesParsing: ResizeArray<Resource> with get, set
            abstract _boundLoadResource: (Resource -> Function -> unit) with get, set
            abstract _queue: obj option with get, set
            abstract resources: ResourceDictionary with get, set
            abstract onProgress: MiniSignal with get, set
            abstract onError: MiniSignal with get, set
            abstract onLoad: MiniSignal with get, set
            abstract onStart: MiniSignal with get, set
            abstract onComplete: MiniSignal with get, set
            abstract concurrency: float with get, set
            abstract add: [<ParamArray>] ``params``: ResizeArray<obj option> -> Loader
            abstract add: name: string * url: string * ?options: LoaderOptions * ?cb: Function -> Loader
            abstract add: obj: U3<string, obj option, ResizeArray<obj option>> * ?options: LoaderOptions * ?cb: Function -> Loader
            abstract pre: fn: Function -> Loader
            abstract ``use``: fn: Function -> Loader
            abstract reset: unit -> Loader
            abstract load: ?cb: Function -> Loader
            abstract _prepareUrl: url: string -> string
            abstract _loadResource: resource: Resource * dequeue: Function -> unit
            abstract _onStart: unit -> unit
            abstract _onComplete: unit -> unit
            abstract _onLoad: resource: Resource -> unit
            abstract destroy: unit -> unit
            [<Emit "$0.on('complete',$1,$2)">] abstract on_complete: fn: (Loaders.Loader -> obj option -> unit) * ?context: obj option -> Loader
            [<Emit "$0.on('error',$1,$2)">] abstract on_error: fn: (Error -> Loaders.Loader -> Resource -> unit) * ?context: obj option -> Loader
            abstract on: ``event``: U2<string, string> * fn: (Loaders.Loader -> Resource -> unit) * ?context: obj option -> Loader
            [<Emit "$0.on('start',$1,$2)">] abstract on_start: fn: (Loaders.Loader -> unit) * ?context: obj option -> Loader
            [<Emit "$0.once('complete',$1,$2)">] abstract once_complete: fn: (Loaders.Loader -> obj option -> unit) * ?context: obj option -> Loader
            [<Emit "$0.once('error',$1,$2)">] abstract once_error: fn: (Error -> Loaders.Loader -> Resource -> unit) * ?context: obj option -> Loader
            abstract once: ``event``: U2<string, string> * fn: (Loaders.Loader -> Resource -> unit) * ?context: obj option -> Loader
            [<Emit "$0.once('start',$1,$2)">] abstract once_start: fn: (Loaders.Loader -> unit) * ?context: obj option -> Loader
            abstract off: ``event``: U6<string, string, string, string, string, string> * ?fn: Function * ?context: obj option -> Loader

        type [<AllowNullLiteral>] LoaderStatic =
            abstract addPixiMiddleware: fn: Function -> unit
            [<Emit "new $0($1...)">] abstract Create: ?baseUrl: string * ?concurrency: float -> Loader

        type [<AllowNullLiteral>] TextureDictionary =
            [<Emit "$0[$1]{{=$2}}">] abstract Item: index: string -> PIXI.Texture with get, set

        type [<AllowNullLiteral>] Resource =
            abstract _flags: float with get, set
            abstract name: string with get, set
            abstract url: string with get, set
            abstract extension: string with get, set
            abstract data: obj option with get, set
            abstract crossOrigin: U2<bool, string> with get, set
            abstract loadType: float with get, set
            abstract xhrType: string with get, set
            abstract metadata: obj option with get, set
            abstract error: Error with get, set
            abstract xhr: XMLHttpRequest option with get, set
            abstract children: ResizeArray<Resource> with get, set
            abstract ``type``: float with get, set
            abstract progressChunk: float with get, set
            abstract _dequeue: Function with get, set
            abstract _onLoadBinding: Function with get, set
            abstract _boundComplete: Function with get, set
            abstract _boundOnError: Function with get, set
            abstract _boundOnProgress: Function with get, set
            abstract _boundXhrOnError: Function with get, set
            abstract _boundXhrOnAbort: Function with get, set
            abstract _boundXhrOnLoad: Function with get, set
            abstract _boundXdrOnTimeout: Function with get, set
            abstract onStart: MiniSignal with get, set
            abstract onProgress: MiniSignal with get, set
            abstract onComplete: MiniSignal with get, set
            abstract onAfterMiddleware: MiniSignal with get, set
            abstract isDataUrl: bool with get, set
            abstract isComplete: bool with get, set
            abstract isLoading: bool with get, set
            abstract complete: unit -> unit
            abstract abort: ?message: string -> unit
            abstract load: ?cb: Function -> unit
            abstract _hasFlag: flag: float -> bool
            abstract _setFlag: flag: float * value: bool -> unit
            abstract _loadElement: ``type``: string -> unit
            abstract _loadSourceElement: ``type``: string -> unit
            abstract _loadXhr: unit -> unit
            abstract _loadXdr: unit -> unit
            abstract _createSource: ``type``: string * url: string * ?mime: string -> HTMLSourceElement
            abstract _onError: ?``event``: obj option -> unit
            abstract _onProgress: ?``event``: obj option -> unit
            abstract _xhrOnError: unit -> unit
            abstract _xhrOnAbort: unit -> unit
            abstract _xdrOnTimeout: unit -> unit
            abstract _xhrOnLoad: unit -> unit
            abstract _determineCrossOrigin: url: string * loc: obj option -> string
            abstract _determineXhrType: unit -> float
            abstract _determineLoadType: unit -> float
            abstract _getExtension: unit -> string
            abstract _getMimeXhrType: ``type``: float -> string
            abstract STATUS_FLAGS: obj with get, set
            abstract TYPE: obj with get, set
            abstract LOAD_TYPE: obj with get, set
            abstract XHR_RESPONSE_TYPE: obj with get, set
            abstract EMPTY_GIF: string with get, set
            abstract texture: Texture with get, set
            abstract spineAtlas: obj option with get, set
            abstract spineData: obj option with get, set
            abstract textures: TextureDictionary option with get, set

        type [<AllowNullLiteral>] ResourceStatic =
            abstract setExtensionLoadType: extname: string * loadType: float -> unit
            abstract setExtensionXhrType: extname: string * xhrType: string -> unit
            [<Emit "new $0($1...)">] abstract Create: name: string * url: U2<string, ResizeArray<string>> * ?options: LoaderOptions -> Resource

    module Mesh =

        type [<AllowNullLiteral>] IExports =
            abstract Mesh: MeshStatic
            abstract CanvasMeshRenderer: CanvasMeshRendererStatic
            abstract MeshRenderer: MeshRendererStatic
            abstract Plane: PlaneStatic
            abstract NineSlicePlane: NineSlicePlaneStatic
            abstract Rope: RopeStatic

        type [<AllowNullLiteral>] Mesh =
            inherit Container
            abstract _texture: Texture with get, set
            abstract uvs: Float32Array with get, set
            abstract vertices: Float32Array with get, set
            abstract indices: Uint16Array with get, set
            abstract dirty: float with get, set
            abstract indexDirty: float with get, set
            abstract vertexDirty: float with get, set
            abstract autoUpdate: bool with get, set
            abstract dirtyVertex: bool with get, set
            abstract _geometryVersion: float with get, set
            abstract blendMode: float with get, set
            abstract pluginName: string with get, set
            abstract canvasPadding: float with get, set
            abstract drawMode: float with get, set
            abstract texture: Texture with get, set
            abstract tintRgb: Float32Array with get, set
            abstract _glDatas: obj with get, set
            abstract _uvTransform: Extras.TextureMatrix with get, set
            abstract uploadUvTransform: bool with get, set
            abstract multiplyUvs: unit -> unit
            abstract refresh: ?forceUpdate: bool -> unit
            abstract _refresh: unit -> unit
            abstract _renderWebGL: renderer: WebGLRenderer -> unit
            abstract _renderCanvas: renderer: CanvasRenderer -> unit
            abstract _onTextureUpdate: unit -> unit
            abstract _calculateBounds: unit -> unit
            abstract containsPoint: point: Point -> bool
            abstract tint: float with get, set
            abstract DRAW_MODES: obj with get, set

        type [<AllowNullLiteral>] MeshStatic =
            [<Emit "new $0($1...)">] abstract Create: texture: Texture * ?vertices: Float32Array * ?uvs: Float32Array * ?indices: Uint16Array * ?drawMode: float -> Mesh

        type [<AllowNullLiteral>] CanvasMeshRenderer =
            abstract renderer: CanvasRenderer with get, set
            abstract render: mesh: Mesh -> unit
            abstract _renderTriangleMesh: mesh: Mesh -> unit
            abstract _renderTriangles: mesh: Mesh -> unit
            abstract _renderDrawTriangle: mesh: Mesh * index0: float * index1: float * index2: float -> unit
            abstract renderMeshFlat: mesh: Mesh -> unit
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] CanvasMeshRendererStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: CanvasRenderer -> CanvasMeshRenderer

        type [<AllowNullLiteral>] MeshRenderer =
            inherit ObjectRenderer
            abstract shader: Shader with get, set
            abstract render: mesh: Mesh -> unit

        type [<AllowNullLiteral>] MeshRendererStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> MeshRenderer

        type [<AllowNullLiteral>] Plane =
            inherit Mesh
            abstract _ready: bool with get, set
            abstract verticesX: float with get, set
            abstract verticesY: float with get, set
            abstract drawMode: float with get, set
            abstract refresh: unit -> unit
            abstract _onTexureUpdate: unit -> unit

        type [<AllowNullLiteral>] PlaneStatic =
            [<Emit "new $0($1...)">] abstract Create: texture: Texture * ?verticesX: float * ?verticesY: float -> Plane

        type [<AllowNullLiteral>] NineSlicePlane =
            inherit Plane
            abstract width: float with get, set
            abstract height: float with get, set
            abstract leftWidth: float with get, set
            abstract rightWidth: float with get, set
            abstract topHeight: float with get, set
            abstract bottomHeight: float with get, set
            abstract _leftWidth: float with get, set
            abstract _rightWidth: float with get, set
            abstract _topHeight: float with get, set
            abstract _bottomHeight: float with get, set
            abstract _height: float with get, set
            abstract _width: float with get, set
            abstract _origHeight: float with get, set
            abstract _origWidth: float with get, set
            abstract _uvh: float with get, set
            abstract _uvw: float with get, set
            abstract updateHorizontalVertices: unit -> unit
            abstract updateVerticalVertices: unit -> unit
            abstract drawSegment: context: U2<CanvasRenderingContext2D, WebGLRenderingContext> * textureSource: obj option * w: float * h: float * x1: float * y1: float * x2: float * y2: float -> unit
            abstract _refresh: unit -> unit

        type [<AllowNullLiteral>] NineSlicePlaneStatic =
            [<Emit "new $0($1...)">] abstract Create: texture: Texture * ?leftWidth: float * ?topHeight: float * ?rightWidth: float * ?bottomHeight: float -> NineSlicePlane

        type [<AllowNullLiteral>] Rope =
            inherit Mesh
            abstract points: ResizeArray<Point> with get, set
            abstract colors: ResizeArray<float> with get, set
            abstract autoUpdate: bool with get, set
            abstract _refresh: unit -> unit
            abstract refreshVertices: unit -> unit

        type [<AllowNullLiteral>] RopeStatic =
            [<Emit "new $0($1...)">] abstract Create: texture: Texture * points: ResizeArray<Point> -> Rope

    module Particles =

        type [<AllowNullLiteral>] IExports =
            abstract ParticleContainer: ParticleContainerStatic
            abstract ParticleBuffer: ParticleBufferStatic
            abstract ParticleRenderer: ParticleRendererStatic

        type [<AllowNullLiteral>] ParticleContainerProperties =
            /// DEPRECIATED - Use `vertices`
            abstract scale: bool option with get, set
            abstract vertices: bool option with get, set
            abstract position: bool option with get, set
            abstract rotation: bool option with get, set
            abstract uvs: bool option with get, set
            abstract tint: bool option with get, set
            abstract alpha: bool option with get, set

        type [<AllowNullLiteral>] ParticleContainer =
            inherit Container
            abstract _tint: float with get, set
            abstract tintRgb: U2<float, ResizeArray<obj option>> with get, set
            abstract tint: float with get, set
            abstract _properties: ResizeArray<bool> with get, set
            abstract _maxSize: float with get, set
            abstract _batchSize: float with get, set
            abstract _glBuffers: obj with get, set
            abstract _bufferUpdateIDs: ResizeArray<float> with get, set
            abstract _updateID: float with get, set
            abstract interactiveChildren: bool with get, set
            abstract blendMode: float with get, set
            abstract autoSize: bool with get, set
            abstract roundPixels: bool with get, set
            abstract baseTexture: BaseTexture with get, set
            abstract setProperties: properties: ParticleContainerProperties -> unit
            abstract onChildrenChange: (float -> unit) with get, set
            abstract destroy: ?options: U2<DestroyOptions, bool> -> unit

        type [<AllowNullLiteral>] ParticleContainerStatic =
            [<Emit "new $0($1...)">] abstract Create: ?maxSize: float * ?properties: ParticleContainerProperties * ?batchSize: float * ?autoResize: bool -> ParticleContainer

        type [<AllowNullLiteral>] ParticleBuffer =
            abstract gl: WebGLRenderingContext with get, set
            abstract size: float with get, set
            abstract dynamicProperties: ResizeArray<obj option> with get, set
            abstract staticProperties: ResizeArray<obj option> with get, set
            abstract staticStride: float with get, set
            abstract staticBuffer: obj option with get, set
            abstract staticData: obj option with get, set
            abstract staticDataUint32: obj option with get, set
            abstract dynamicStride: float with get, set
            abstract dynamicBuffer: obj option with get, set
            abstract dynamicData: obj option with get, set
            abstract dynamicDataUint32: obj option with get, set
            abstract _updateID: float with get, set
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] ParticleBufferStatic =
            [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * properties: obj option * dynamicPropertyFlags: ResizeArray<obj option> * size: float -> ParticleBuffer

        type [<AllowNullLiteral>] ParticleRendererProperty =
            abstract attribute: float with get, set
            abstract size: float with get, set
            abstract uploadFunction: children: ResizeArray<PIXI.DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract unsignedByte: obj option with get, set
            abstract offset: float with get, set

        type [<AllowNullLiteral>] ParticleRenderer =
            inherit ObjectRenderer
            abstract shader: GlCore.GLShader with get, set
            abstract indexBuffer: WebGLBuffer with get, set
            abstract properties: ResizeArray<ParticleRendererProperty> with get, set
            abstract tempMatrix: Matrix with get, set
            abstract start: unit -> unit
            abstract generateBuffers: container: ParticleContainer -> ResizeArray<ParticleBuffer>
            abstract _generateOneMoreBuffer: container: ParticleContainer -> ParticleBuffer
            abstract uploadVertices: children: ResizeArray<DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract uploadPosition: children: ResizeArray<DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract uploadRotation: children: ResizeArray<DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract uploadUvs: children: ResizeArray<DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract uploadTint: children: ResizeArray<DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract uploadAlpha: children: ResizeArray<DisplayObject> * startIndex: float * amount: float * array: ResizeArray<float> * stride: float * offset: float -> unit
            abstract destroy: unit -> unit
            abstract indices: Uint16Array with get, set

        type [<AllowNullLiteral>] ParticleRendererStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> ParticleRenderer

    module Prepare =

        type [<AllowNullLiteral>] IExports =
            abstract BasePrepare: BasePrepareStatic
            abstract CanvasPrepare: CanvasPrepareStatic
            abstract WebGLPrepare: WebGLPrepareStatic
            abstract CountLimiter: CountLimiterStatic
            abstract TimeLimiter: TimeLimiterStatic

        type [<AllowNullLiteral>] AddHook =
            [<Emit "$0($1...)">] abstract Invoke: item: obj option * queue: ResizeArray<obj option> -> bool

        type [<AllowNullLiteral>] UploadHook<'UploadHookSource> =
            [<Emit "$0($1...)">] abstract Invoke: prepare: 'UploadHookSource * item: obj option -> bool

        type [<AllowNullLiteral>] BasePrepare<'UploadHookSource> =
            abstract limiter: U2<CountLimiter, TimeLimiter> with get, set
            abstract renderer: SystemRenderer with get, set
            abstract uploadHookHelper: 'UploadHookSource with get, set
            abstract queue: ResizeArray<obj option> with get, set
            abstract addHooks: ResizeArray<AddHook> with get, set
            abstract uploadHooks: Array<UploadHook<'UploadHookSource>> with get, set
            abstract completes: ResizeArray<Function> with get, set
            abstract ticking: bool with get, set
            abstract delayedTick: (unit -> unit) with get, set
            abstract upload: item: U8<Function, DisplayObject, Container, BaseTexture, Texture, Graphics, Text, obj option> * ?``done``: (unit -> unit) -> unit
            abstract tick: unit -> unit
            abstract prepareItems: unit -> unit
            abstract registerFindHook: addHook: AddHook -> BasePrepare<'UploadHookSource>
            abstract registerUploadHook: uploadHook: UploadHook<'UploadHookSource> -> BasePrepare<'UploadHookSource>
            abstract findMultipleBaseTextures: item: PIXI.DisplayObject * queue: ResizeArray<obj option> -> bool
            abstract findBaseTexture: item: PIXI.DisplayObject * queue: ResizeArray<obj option> -> bool
            abstract findTexture: item: PIXI.DisplayObject * queue: ResizeArray<obj option> -> bool
            abstract add: item: U7<PIXI.DisplayObject, PIXI.Container, PIXI.BaseTexture, PIXI.Texture, PIXI.Graphics, PIXI.Text, obj option> -> BasePrepare<'UploadHookSource>
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] BasePrepareStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: SystemRenderer -> BasePrepare<'UploadHookSource>

        type [<AllowNullLiteral>] CanvasPrepare =
            inherit BasePrepare<CanvasPrepare>
            abstract canvas: HTMLCanvasElement with get, set
            abstract ctx: CanvasRenderingContext2D with get, set

        type [<AllowNullLiteral>] CanvasPrepareStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: CanvasRenderer -> CanvasPrepare

        type [<AllowNullLiteral>] WebGLPrepare =
            inherit BasePrepare<WebGLRenderer>

        type [<AllowNullLiteral>] WebGLPrepareStatic =
            [<Emit "new $0($1...)">] abstract Create: renderer: WebGLRenderer -> WebGLPrepare

        type [<AllowNullLiteral>] CountLimiter =
            abstract maxItemsPerFrame: float with get, set
            abstract itemsLeft: float with get, set
            abstract beginFrame: unit -> unit
            abstract allowedToUpload: unit -> bool

        type [<AllowNullLiteral>] CountLimiterStatic =
            [<Emit "new $0($1...)">] abstract Create: maxItemsPerFrame: float -> CountLimiter

        type [<AllowNullLiteral>] TimeLimiter =
            abstract maxMilliseconds: float with get, set
            abstract frameStart: float with get, set
            abstract beginFrame: unit -> unit
            abstract allowedToUpload: unit -> bool

        type [<AllowNullLiteral>] TimeLimiterStatic =
            [<Emit "new $0($1...)">] abstract Create: maxMilliseconds: float -> TimeLimiter

    module GlCore =

        type [<AllowNullLiteral>] IExports =
            abstract createContext: view: HTMLCanvasElement * ?options: ContextOptions -> WebGLRenderingContext
            abstract setVertexAttribArrays: gl: WebGLRenderingContext * attribs: ResizeArray<Attrib> * ?state: WebGLState -> WebGLRenderingContext option
            abstract GLBuffer: GLBufferStatic
            abstract GLFramebuffer: GLFramebufferStatic
            abstract GLShader: GLShaderStatic
            abstract GLTexture: GLTextureStatic
            abstract VertexArrayObject: VertexArrayObjectStatic

        type [<AllowNullLiteral>] ContextOptions =
            /// Boolean that indicates if the canvas contains an alpha buffer.
            abstract alpha: bool option with get, set
            /// Boolean that indicates that the drawing buffer has a depth buffer of at least 16 bits.
            abstract depth: bool option with get, set
            /// Boolean that indicates that the drawing buffer has a stencil buffer of at least 8 bits.
            abstract stencil: bool option with get, set
            /// Boolean that indicates whether or not to perform anti-aliasing.
            abstract antialias: bool option with get, set
            /// Boolean that indicates that the page compositor will assume the drawing buffer contains colors with pre-multiplied alpha.
            abstract premultipliedAlpha: bool option with get, set
            /// If the value is true the buffers will not be cleared and will preserve their values until cleared or overwritten by the author.
            abstract preserveDrawingBuffer: bool option with get, set
            /// Boolean that indicates if a context will be created if the system performance is low.
            abstract failIfMajorPerformanceCaveat: bool option with get, set

        type [<AllowNullLiteral>] GLBuffer =
            abstract _updateID: float option with get, set
            abstract gl: WebGLRenderingContext with get, set
            abstract buffer: WebGLBuffer with get, set
            abstract ``type``: float with get, set
            abstract drawType: float with get, set
            abstract data: U3<ArrayBuffer, ArrayBufferView, obj option> with get, set
            abstract upload: ?data: U3<ArrayBuffer, ArrayBufferView, obj option> * ?offset: float * ?dontBind: bool -> unit
            abstract bind: unit -> unit
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] GLBufferStatic =
            [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * ``type``: float * data: U3<ArrayBuffer, ArrayBufferView, obj option> * drawType: float -> GLBuffer
            abstract createVertexBuffer: gl: WebGLRenderingContext * data: U3<ArrayBuffer, ArrayBufferView, obj option> * drawType: float -> GLBuffer
            abstract createIndexBuffer: gl: WebGLRenderingContext * data: U3<ArrayBuffer, ArrayBufferView, obj option> * drawType: float -> GLBuffer
            abstract create: gl: WebGLRenderingContext * ``type``: float * data: U3<ArrayBuffer, ArrayBufferView, obj option> * drawType: float -> GLBuffer

        type [<AllowNullLiteral>] GLFramebuffer =
            abstract gl: WebGLRenderingContext with get, set
            abstract frameBuffer: WebGLFramebuffer with get, set
            abstract stencil: WebGLRenderbuffer with get, set
            abstract texture: GLTexture with get, set
            abstract width: float with get, set
            abstract height: float with get, set
            abstract enableTexture: texture: GLTexture -> unit
            abstract enableStencil: unit -> unit
            abstract clear: r: float * g: float * b: float * a: float -> unit
            abstract bind: unit -> unit
            abstract unbind: unit -> unit
            abstract resize: width: float * height: float -> unit
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] GLFramebufferStatic =
            [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * width: float * height: float -> GLFramebuffer
            abstract createRGBA: gl: WebGLRenderingContext * width: float * height: float * data: U3<ArrayBuffer, ArrayBufferView, obj option> -> GLFramebuffer
            abstract createFloat32: gl: WebGLRenderingContext * width: float * height: float * data: U3<ArrayBuffer, ArrayBufferView, obj option> -> GLFramebuffer

        type [<AllowNullLiteral>] GLShader =
            abstract gl: WebGLRenderingContext with get, set
            abstract program: WebGLProgram option with get, set
            abstract uniformData: obj option with get, set
            abstract uniforms: obj option with get, set
            abstract attributes: obj option with get, set
            abstract bind: unit -> GLShader
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] GLShaderStatic =
            [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * vertexSrc: U2<string, ResizeArray<string>> * fragmentSrc: U2<string, ResizeArray<string>> * ?precision: string * ?attributeLocations: GLShaderStaticAttributeLocations -> GLShader

        type [<AllowNullLiteral>] GLShaderStaticAttributeLocations =
            [<Emit "$0[$1]{{=$2}}">] abstract Item: key: string -> float with get, set

        type [<AllowNullLiteral>] GLTexture =
            abstract gl: WebGLRenderingContext with get, set
            abstract texture: WebGLTexture with get, set
            abstract mipmap: bool with get, set
            abstract premultiplyAlpha: bool with get, set
            abstract width: float with get, set
            abstract height: float with get, set
            abstract format: float with get, set
            abstract ``type``: float with get, set
            abstract upload: source: U4<HTMLImageElement, ImageData, HTMLVideoElement, HTMLCanvasElement> -> unit
            abstract uploadData: data: U2<ArrayBuffer, ArrayBufferView> * width: float * height: float -> unit
            abstract bind: ?location: float -> unit
            abstract unbind: unit -> unit
            abstract minFilter: linear: bool -> unit
            abstract magFilter: linear: bool -> unit
            abstract enableMipmap: unit -> unit
            abstract enableLinearScaling: unit -> unit
            abstract enableNearestScaling: unit -> unit
            abstract enableWrapClamp: unit -> unit
            abstract enableWrapRepeat: unit -> unit
            abstract enableWrapMirrorRepeat: unit -> unit
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] GLTextureStatic =
            [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * ?width: float * ?height: float * ?format: float * ?``type``: float -> GLTexture
            abstract fromSource: gl: WebGLRenderingContext * source: U4<HTMLImageElement, ImageData, HTMLVideoElement, HTMLCanvasElement> * ?premultipleAlpha: bool -> GLTexture
            abstract fromData: gl: WebGLRenderingContext * data: ResizeArray<float> * width: float * height: float -> GLTexture

        type [<AllowNullLiteral>] Attrib =
            abstract attribute: obj with get, set
            abstract normalized: bool with get, set
            abstract stride: float with get, set
            abstract start: float with get, set
            abstract buffer: ArrayBuffer with get, set

        type [<AllowNullLiteral>] WebGLRenderingContextAttribute =
            abstract buffer: WebGLBuffer with get, set
            abstract attribute: obj option with get, set
            abstract ``type``: float with get, set
            abstract normalized: bool with get, set
            abstract stride: float with get, set
            abstract start: float with get, set

        type [<AllowNullLiteral>] AttribState =
            abstract tempAttribState: ResizeArray<Attrib> with get, set
            abstract attribState: ResizeArray<Attrib> with get, set

        type [<AllowNullLiteral>] VertexArrayObject =
            abstract FORCE_NATIVE: bool with get, set
            abstract nativeVaoExtension: obj option with get, set
            abstract nativeState: AttribState with get, set
            abstract nativeVao: VertexArrayObject with get, set
            abstract gl: WebGLRenderingContext with get, set
            abstract attributes: ResizeArray<Attrib> with get, set
            abstract indexBuffer: GLBuffer with get, set
            abstract dirty: bool with get, set
            abstract bind: unit -> VertexArrayObject
            abstract unbind: unit -> VertexArrayObject
            abstract activate: unit -> VertexArrayObject
            abstract addAttribute: buffer: GLBuffer * attribute: Attrib * ?``type``: float * ?normalized: bool * ?stride: float * ?start: float -> VertexArrayObject
            abstract addIndex: buffer: GLBuffer * ?options: obj option -> VertexArrayObject
            abstract clear: unit -> VertexArrayObject
            abstract draw: ``type``: float * size: float * start: float -> VertexArrayObject
            abstract destroy: unit -> unit

        type [<AllowNullLiteral>] VertexArrayObjectStatic =
            [<Emit "new $0($1...)">] abstract Create: gl: WebGLRenderingContext * ?state: WebGLState -> VertexArrayObject

    type [<AllowNullLiteral>] DecomposedDataUri =
        abstract mediaType: string with get, set
        abstract subType: string with get, set
        abstract encoding: string with get, set
        abstract data: obj option with get, set

    module Utils =
        let [<Import("isMobile","pixi.js/utils")>] isMobile: IsMobile.IExports = jsNative

        type [<AllowNullLiteral>] IExports =
            abstract uid: unit -> float
            abstract hex2rgb: hex: float * ?out: ResizeArray<float> -> ResizeArray<float>
            abstract hex2string: hex: float -> string
            abstract rgb2hex: rgb: ResizeArray<float> -> float
            abstract canUseNewCanvasBlendModes: unit -> bool
            abstract getResolutionOfUrl: url: string * ?defaultValue: float -> float
            abstract getSvgSize: svgString: string -> obj option
            abstract decomposeDataUri: dataUri: string -> U2<DecomposedDataUri, unit>
            abstract getUrlFileExtension: url: string -> U2<string, unit>
            abstract sayHello: ``type``: string -> unit
            abstract skipHello: unit -> unit
            abstract isWebGLSupported: unit -> bool
            abstract sign: n: float -> float
            abstract removeItems: arr: ResizeArray<'T> * startIdx: float * removeCount: float -> unit
            abstract correctBlendMode: blendMode: float * premultiplied: bool -> float
            abstract clearTextureCache: unit -> unit
            abstract destroyTextureCache: unit -> unit
            abstract premultiplyTint: tint: float * alpha: float -> float
            abstract premultiplyRgba: rgb: U2<Float32Array, ResizeArray<float>> * alpha: float * ?out: Float32Array * ?premultiply: bool -> Float32Array
            abstract premultiplyTintToRgba: tint: float * alpha: float * ?out: Float32Array * ?premultiply: bool -> Float32Array
            abstract premultiplyBlendMode: ResizeArray<ResizeArray<float>>
            abstract TextureCache: obj option
            abstract BaseTextureCache: obj option
            abstract EventEmitter: EventEmitterStatic

        module IsMobile =

            type [<AllowNullLiteral>] IExports =
                abstract apple: obj
                abstract android: obj
                abstract amazon: obj
                abstract windows: obj
                abstract seven_inch: bool
                abstract other: obj
                abstract any: bool
                abstract phone: bool
                abstract tablet: bool

        type [<AllowNullLiteral>] EventEmitter =
            abstract prefixed: U2<string, bool> with get, set
            abstract EventEmitter: obj with get, set
            /// Return an array listing the events for which the emitter has registered listeners.
            abstract eventNames: unit -> Array<U2<string, Symbol>>
            /// <summary>Return the listeners registered for a given event.</summary>
            /// <param name="event">The event name.</param>
            abstract listeners: ``event``: U2<string, Symbol> -> ResizeArray<Function>
            /// <summary>Check if there listeners for a given event.
            /// If `exists` argument is not `true` lists listeners.</summary>
            /// <param name="event">The event name.</param>
            /// <param name="exists">Only check if there are listeners.</param>
            abstract listeners: ``event``: U2<string, Symbol> * exists: bool -> bool
            /// <summary>Calls each of the listeners registered for a given event.</summary>
            /// <param name="event">The event name.</param>
            /// <param name="args">Arguments that are passed to registered listeners</param>
            abstract emit: ``event``: U2<string, Symbol> * [<ParamArray>] args: ResizeArray<obj option> -> bool
            /// <summary>Add a listener for a given event.</summary>
            /// <param name="event">The event name.</param>
            /// <param name="fn">The listener function.</param>
            /// <param name="context">The context to invoke the listener with.</param>
            abstract on: ``event``: U2<string, Symbol> * fn: Function * ?context: obj option -> EventEmitter
            /// <summary>Add a one-time listener for a given event.</summary>
            /// <param name="event">The event name.</param>
            /// <param name="fn">The listener function.</param>
            /// <param name="context">The context to invoke the listener with.</param>
            abstract once: ``event``: U2<string, Symbol> * fn: Function * ?context: obj option -> EventEmitter
            /// <summary>Remove the listeners of a given event.</summary>
            /// <param name="event">The event name.</param>
            /// <param name="fn">Only remove the listeners that match this function.</param>
            /// <param name="context">Only remove the listeners that have this context.</param>
            /// <param name="once">Only remove one-time listeners.</param>
            abstract removeListener: ``event``: U2<string, Symbol> * ?fn: Function * ?context: obj option * ?once: bool -> EventEmitter
            /// <summary>Remove all listeners, or those of the specified event.</summary>
            /// <param name="event">The event name.</param>
            abstract removeAllListeners: ?``event``: U2<string, Symbol> -> EventEmitter
            /// Alias method for `removeListener`
            abstract off: ``event``: U2<string, Symbol> * ?fn: Function * ?context: obj option * ?once: bool -> EventEmitter
            /// Alias method for `on`
            abstract addListener: ``event``: U2<string, Symbol> * fn: Function * ?context: obj option -> EventEmitter
            /// This function doesn"t apply anymore.
            abstract setMaxListeners: unit -> EventEmitter

        type [<AllowNullLiteral>] EventEmitterStatic =
            /// Minimal EventEmitter interface that is molded against the Node.js
            /// EventEmitter interface.
            [<Emit "new $0($1...)">] abstract Create: unit -> EventEmitter

    module Core =

        type SpriteBatch =
            ParticleContainer

        type AssetLoader =
            Loaders.Loader

        type Stage =
            Container

        type DisplayObjectContainer =
            Container

        type Strip =
            Mesh.Mesh

        type Rope =
            Mesh.Rope

        type ParticleContainer =
            Particles.ParticleContainer

        type MovieClip =
            Extras.AnimatedSprite

        type TilingSprite =
            Extras.TilingSprite

        type BaseTextureCache =
            obj option

        type BitmapText =
            Extras.BitmapText

        type math =
            obj option

        type AbstractFilter<'U> =
            Filter<'U>

        type TransformManual =
            TransformBase

        type TARGET_FPMS =
            float

        type FILTER_RESOLUTION =
            float

        type RESOLUTION =
            float

        type MIPMAP_TEXTURES =
            obj option

        type SPRITE_BATCH_SIZE =
            float

        type SPRITE_MAX_TEXTURES =
            float

        type RETINA_PREFIX =
            U2<RegExp, string>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module RETINA_PREFIX =
            let ofRegExp v: RETINA_PREFIX = v |> U2.Case1
            let isRegExp (v: RETINA_PREFIX) = match v with U2.Case1 _ -> true | _ -> false
            let asRegExp (v: RETINA_PREFIX) = match v with U2.Case1 o -> Some o | _ -> None
            let ofString v: RETINA_PREFIX = v |> U2.Case2
            let isString (v: RETINA_PREFIX) = match v with U2.Case2 _ -> true | _ -> false
            let asString (v: RETINA_PREFIX) = match v with U2.Case2 o -> Some o | _ -> None

        type DEFAULT_RENDER_OPTIONS =
            float

        type PRECISION =
            string

module Pixi =

    type [<AllowNullLiteral>] IExports =
        abstract gl: obj