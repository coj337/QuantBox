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
            triangleArbMatrix: [],
            normalArbMatrix: []
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
        this.state.hubConnection.on('ReceiveTriangleArbitrage', (receivedMessage) => {
            var arbitrage = receivedMessage;
            this.setState((prevState, props) => ({
                triangleArbMatrix: prevState.triangleArbMatrix.push(arbitrage)
            }));
        });

        this.state.hubConnection.on('ReceiveNormalArbitrage', (receivedMessage) => {
            var arbitrage = receivedMessage;
            this.setState({
                normalArbMatrix: arbitrage
            });
        });
    }

    render() {
        return (
            <Row>
                <div>Triangle Arbs</div>
                {this.state.triangleArbMatrix.map((arbitrage, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <ArbitragePanel
                            pair={arbitrage.pair}
                            startExchange={arbitrage.startExchange}
                            endExchange={arbitrage.endExchange}
                            spread={arbitrage.spread}
                        />
                    </Col>
                ))}

                <div>Two-Way Arbs</div>
                {this.state.normalArbMatrix.map((arbitrage, i) => (
                    <Col xs={6} md={4} lg={3} key={i}>
                        <ArbitragePanel
                            exchange={arbitrage.exchange}
                            path={arbitrage.path}
                            transactionFee={arbitrage.transactionFee}
                            networkFee={arbitrage.networkFee}
                            timePerLoop={arbitrage.timePerLoop}
                        />
                    </Col>
                ))}
            </Row>
        );
    }
}
