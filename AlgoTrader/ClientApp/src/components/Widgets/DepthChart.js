import React, { Component } from 'react';
import { Widget } from './Widget';

export class DepthChart extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Widget
                title={"Market Depth"}
                minH={2}
                minW={2}
                currentH={2}
                currentW={2}
                x={0}
                y={2}
            >
                Depth Stuff
            </Widget>
        );
    }
}