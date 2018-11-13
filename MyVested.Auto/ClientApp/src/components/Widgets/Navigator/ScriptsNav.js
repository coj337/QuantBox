import React, { Component } from 'react';

export class ScriptsNav extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            scripts: []
        };
    }

    componentDidMount() {
        fetch("/Nav/Scripts")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        scripts: result
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
        const { error, isLoaded, scripts } = this.state;
        const scriptsList = scripts.map((script) =>
            <li>{script.name}</li>
        );

        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <ul>{scriptsList}</ul>
            );
        }
    }
}