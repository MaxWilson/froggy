import { Sprite, Stage, Text } from "react-pixi-fiber";
import * as React from "react";
import * as PropTypes from "prop-types";
import Select from "react-select";
import * as PIXI from "pixi.js";

const bunny = "https://i.imgur.com/IaUrttj.png";
const centerAnchor = new PIXI.Point(0.5, 0.5);

function Bunny(props:any) {
  return (
    <Sprite
      anchor={centerAnchor}
      texture={PIXI.Texture.fromImage(bunny)}
      {...props}
    />
  );
}

class RotationState {
  rotation: number
}

// http://pixijs.io/examples/#/basics/basic.js
export class RotatingBunny extends React.Component<any, RotationState, {}> {
  state: RotationState = {
    rotation: 0
  };

  componentDidMount() {
    this.context.app.ticker.add(this.animate);
  }

  componentWillUnmount() {
    this.context.app.ticker.remove(this.animate);
  }

  animate = (delta: number) => {
    // just for fun, let's rotate mr rabbit a little
    // delta is 1 if running at 100% performance
    // creates frame-independent tranformation
    this.setState(state => ({
      ...state,
      rotation: state.rotation + 0.1 * delta
    }));
  };

  render() {
    return <Bunny {...this.props} rotation={this.state.rotation} />;
  }

  name() {
    return "bunny. :) Bunny!";
  }
  static contextTypes = {
    app: PropTypes.object
  }
}


export function BunnyStage(txt:string="Hello") {
  return (
    <Stage width={800} height={600} options={{ backgroundColor: 0x10bb99 }}>
      <RotatingBunny x={300} y={200} />
      <Text x={200} y={300} text={txt} style={new PIXI.TextStyle({ wordWrap: true, wordWrapWidth: 300 })} />
    </Stage>
  );
}