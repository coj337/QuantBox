import React, { Component } from 'react';
import { Row, Col } from 'react-bootstrap';
import Axios from 'axios';
import { toast } from 'react-toastify';

export class ArbitrageTrades extends Component {
    displayName = ArbitrageTrades.name

    constructor(props) {
        super(props);

        this.state = {
            trades: [],
            tradesLoaded: false,
            botId: this.props.botId
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
        Axios.get('/Bot/' + this.state.botId + '/Trades')
            .then((response) => {
                console.log(response);
                this.setState({
                    trades: response.data,
                    tradesLoaded: true
                });
            })
            .catch((error) => {
                if (error.response.data) {
                    toast.error(error.response.data);
                }
                else {
                    toast.error("Couldn't get trades. (" + error.response.status + " " + error.response.statusText + ")");
                }
            });
    }

    render() {
        return (
            <Col xs={12}>
                <h4 className="subTitle">Recent Trades</h4>
                <Row className="darkerContainer">
                    <Col xs={3}>Path</Col>
                    <Col xs={2}>Estimated Profit</Col>
                    <Col xs={2}>Actual Profit</Col>
                    <Col xs={2}>Time Taken</Col>
                    <Col xs={2}>Order Date/Time</Col>
                    <Col xs={1}>N/A</Col>

                    {this.state.tradesLoaded ?
                        this.state.trades.length > 0 ?
                            this.state.trades.map((trade, i) => {
                                console.log(trade);
                                return <div key={i}>
                                    <Col xs={3}>{trade.trades ? <span>None?</span> : trade.trades[0].marketSymbol + " -> " + trade.trades[1].marketSymbol + " -> " + trade.trades[2].marketSymbol}</Col>
                                    <Col xs={2}>{trade.estimatedProfit}</Col>
                                    <Col xs={2}>{trade.actualProfit}</Col>
                                    <Col xs={2}>{((new Date(trade.timeFinished).getTime() / 1000) - (new Date(trade.timeStarted).getTime() / 1000)).toFixed(4)} seconds</Col>
                                    <Col xs={2}>{trade.timeFinished}</Col>
                                    <Col xs={1}>{}N/A</Col>
                                </div>
                            }) :
                            <Col xs={12}>No trades yet.</Col> :
                        <Col xs={12}>Loading trades...</Col>
                    }
                </Row>
            </Col>
        );
    }
}