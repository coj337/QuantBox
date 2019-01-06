import React, { Component } from 'react';
import { Widget } from './Widget';

export class Orderbook extends Component {
    render() {
        return (
            <Widget
                title={"Orderbook"}
                minH={2}
                minW={2}
                currentH={2}
                currentW={2}
            >
                Orderbook Stuff
            </Widget>
        );
    }
}