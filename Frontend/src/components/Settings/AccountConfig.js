import React, { Component } from 'react';
import './AccountConfig.css';
import { AccountConfigCell } from './AccountConfigCell';

export class AccountConfig extends Component {
    displayName = AccountConfig.name

    constructor(props) {
        super(props);

        this.state = {
            name: props.name,
            disabled: props.disabled,
            active: false,
            publicKey: props.publicKey,
            privateKey: ""
        };

        this.saveKey = this.saveKey.bind(this);
        this.editKey = this.editKey.bind(this);
        this.cancelEdit = this.cancelEdit.bind(this);
        this.deleteKey = this.deleteKey.bind(this);
    }

    saveKey(e) {
        var publicKey = document.getElementById(this.state.name + "PublicKeyInput").value;
        var privateKey = document.getElementById(this.state.name + "PrivateKeyInput").value;

        //TODO: Show a loader
        this.setState({
            active: false,
            publicKey: publicKey,
            privateKey: privateKey
        });

        fetch("/Market/SaveSettings", {
            method: "POST",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: this.state.name,
                publicKey: publicKey,
                privateKey: privateKey
            })
        })//.then(res => res.json())
            .then(
                (result) => {
                    //console.log(result);
                },
                (error) => {
                    console.log(error);
                }
            );
    }

    editKey(e) {
        this.setState({
            active: true
        });
    }

    cancelEdit(e) {
        this.setState({
            active: false
        });
    }

    deleteKey(e) {
        //This will be used once multiple keys can be used for the same exchange
    }

    render() {
        return (
            <div className={`accountConfig${this.state.disabled ? " disabled" : ""}`}>
                <span className="accountName">{this.state.name}</span>
                {this.state.disabled ?
                    <span className="accountMessage">COMING SOON</span> :
                    <span>
                        <span className="keyContainers">
                            <AccountConfigCell private={false} active={this.state.active} text={this.state.publicKey} id={`${this.state.name}PublicKeyInput`}/>
                            <AccountConfigCell private={true} active={this.state.active} id={`${this.state.name}PrivateKeyInput`} />
                        </span>
                        {this.state.active ?
                            <span>
                                <button className="configButton" onClick={this.saveKey}>Save</button>
                                <button className="configButton" onClick={this.cancelEdit}>Cancel</button> 
                            </span>:
                            <button className="configButton" onClick={this.editKey}>Edit</button>
                        }
                        {/*Tick/Cross indicating account status*/}
                        {/*<button onClick={this.deleteKey}>Delete</button>*/}
                    </span>
                }
                
            </div>
        );
    }
}