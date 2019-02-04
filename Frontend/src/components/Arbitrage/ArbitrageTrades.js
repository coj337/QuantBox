import React, { Component } from 'react';
import { Col } from 'react-bootstrap';

export class ArbitrageTrades extends Component {
    displayName = ArbitrageTrades.name

    constructor(props) {
        super(props);

        this.state = {

        };
    }

    componentDidMount() {
        this.getItems();
        this.timer = setInterval(() => this.getItems(), 30 * 1000); //Polling until I get websockets to work
    }

    componentWillUnmount() {
        clearInterval(this.timer);
        this.timer = null;
    }

    getItems() {

    }

    render() {
        return (
            <Col xs={12}>
                <h4 className="subTitle">Recent Trades</h4>
                <div className="darkerContainer">TBD</div>
            </Col>
        );
    }
}