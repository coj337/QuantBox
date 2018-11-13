import React, { Component } from 'react';

export class BotsNav extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            bots: []
        };
    }

    componentDidMount() {
        fetch("/Nav/Bots")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        bots: result
                    });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            );
    }

    render() {
        const { error, isLoaded, bots } = this.state;
        const botsList = bots.map((bot) =>
            <li>{bot.name}</li>
        );

        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <ul>{botsList}</ul>
            );
        }
    }
}