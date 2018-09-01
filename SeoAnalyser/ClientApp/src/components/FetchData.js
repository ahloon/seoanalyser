import React, { Component } from 'react';

export class FetchData extends Component {
	static renderForecastsTable(words) {
		return (
			<tbody>
				{words.map((word, index) =>
					<tr key={index}>
						<td>{word.word}</td>
						<td>{word.occurrences}</td>
					</tr>
				)}
			</tbody>
		);
	}

	static renderError(error) {
		if (error !== null) {
			return (
				<div><p>There was an error. {error} </p></div>
			);
		}
	}

	displayName = FetchData.name
	constructor(props) {
		super(props);
		this.state = {
			words: [],
			loading: true,
			text: "The quick brown fox jumps over the lazy dog",
			isUrl: false,
			inPage: false,
			inMeta: false,
			external: false,
			errorMsg: "",
			sort: {
				column: 'word',
				direction: 'asc'
			}
		};
		this.sortBy.bind(this);
		this.handleTextChange = this.handleTextChange.bind(this);
		this.handleChange = this.handleChange.bind(this);
		this.fetchData();
	}

	sortBy(column) {
		const direction = this.state.sort.column ? (this.state.sort.direction === 'asc' ? 'desc' : 'asc') : 'desc';
		const sortedData = this.state.words.sort((a, b) => {
			if (column === 'word') {
				const nameA = a.word.toUpperCase();
				const nameB = b.word.toUpperCase();
				if (nameA < nameB) {
					return -1;
				}
				if (nameA > nameB) {
					return 1;
				}

				return 0;
			} else {
				return a.occurrences - b.occurrences;
			}
		});

		if (direction === 'desc') {
			sortedData.reverse();
		}

		this.setState({
			words: sortedData,
			sort: {
				column,
				direction
			}
		});
	}

	fetchData() {
		const encodedText = this.state.isUrl ? encodeURIComponent(this.state.text) : this.state.text;
		fetch('api/SampleData/AnalyserResults',
			{
				method: 'POST',
				headers: new Headers({
					'Content-Type': 'application/x-www-form-urlencoded'
				}),
				body: `text=${encodedText}&isUrl=${this.state.isUrl}&inPage=${this.state.inPage}&inMeta=${this.state.inMeta}&external=${this.state.external}`
			})
			.then(this.handleErrors)
			.then(response => response.json())
			.then(data => {
				this.setState({ words: data, loading: false, errorMsg: null }, () => { this.sortBy('word') });
			})
			.catch(error => this.setState({ errorMsg: error.message }));
	}

	handleTextChange(event) {
		this.setState({ text: event.target.value }, () => { this.fetchData(); });
	}

	handleChange(event) {
		const target = event.target;
		const name = target.name;
		const value = target.type === 'checkbox' ? target.checked : target.value;
		this.setState({ [name]: value }, () => { this.fetchData(); });

	}

	handleErrors(response) {
		if (!response.ok) {
			console.log(response);
			throw Error(response.statusText);
		}
		return response;
	}

	render() {
		let contents = FetchData.renderForecastsTable(this.state.words);
		const error = FetchData.renderError(this.state.errorMsg);
		return (
			<div>
				<h1>SEO Analyser</h1>
				<p />
				<textarea rows="4" cols="80" name="text" value={this.state.text} onChange={this.handleTextChange} /><br />
				<input type="checkbox" name="isUrl" checked={this.state.isUrl} onChange={this.handleChange} /> This is a URL
				<input type="checkbox" name="inPage" checked={this.state.inPage} onChange={this.handleChange} /> Calculate in page
				<input type="checkbox" name="inMeta" checked={this.state.inMeta} onChange={this.handleChange} /> Calculate in Meta tag
				<input type="checkbox" name="external" checked={this.state.external} onChange={this.handleChange} /> Calculate external links<br />
				{error}
				<table className='table'>
					<thead>
						<tr>
							<th onClick={() => this.sortBy('word')}>Word</th>
							<th onClick={() => this.sortBy('occurrences')}>Number of occurrences</th>
						</tr>
					</thead>
					{contents}
				</table>
			</div>
		);
	}
}
