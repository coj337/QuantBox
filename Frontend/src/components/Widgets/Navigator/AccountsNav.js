import React, { Component } from 'react';

export class AccountsNav extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            accounts: []
        };
    }

    componentDidMount() {
        fetch("/Nav/Accounts")
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        accounts: result
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
        const { error, isLoaded, accounts } = this.state;
        const accountList = accounts.map((account) =>
            <li>{account.name}</li>
        );

        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <ul>{accountList}</ul>
            );
        }
    }
}