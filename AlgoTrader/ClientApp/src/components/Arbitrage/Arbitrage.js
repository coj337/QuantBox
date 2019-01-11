import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { ArbitragePanel } from './ArbitragePanel';

export class Arbitrage extends Component {
    displayName = Arbitrage.name

    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            arbMatrix: []
        };
    }

    componentWillMount() {
        const hubConnection = new HubConnectionBuilder().withUrl('/arbitrageHub').build();
        this.setState({
            hubConnection: hubConnection
        }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :( (' + err + ')'));
        });
    }

    componentDidMount() {
        this.state.hubConnection.on('ReceiveArbitrageMatrix', (receivedMessage) => {
            var arbitrage = receivedMessage;
            this.setState({
                arbMatrix: arbitrage
            });
        });
    }

    render() {
        return (
            <Row>
                {this.state.arbMatrix.map((arbitrage, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <ArbitragePanel
                            pair={arbitrage.pair}
                            startExchange={arbitrage.startExchange}
                            endExchange={arbitrage.endExchange}
                            spread={arbitrage.spread}
                        />
                    </Col>
                ))}
            </Row>
        );
    }
}
