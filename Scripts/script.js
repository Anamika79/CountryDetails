const dropdown = document.getElementById("countryDropdown");
const tableBody = document.getElementById("table_body");

function createDropdownOptions(data) {
    return data.map((country) => `<option value="${country.name.common}">${country.name.common}</option>`);
}


const corsProxyUrl = 'https://cors-anywhere.herokuapp.com/'; //to bypass the Cross-Origin Resource Sharing (CORS) restriction.
const apiUrl = 'https://restcountries.com/v3.1/all';
fetch(corsProxyUrl + apiUrl)
  .then((response) => response.json())
  .then((objectData) => {
    const dropdownOptions = createDropdownOptions(objectData);
    dropdown.innerHTML = '<option value="">Select a country</option>' + dropdownOptions.join('');

    function updateTable() {

      const selectedCountry = dropdown.value;
      if (selectedCountry) {
        const data = objectData.find((country) => country.name.common === selectedCountry);

        let currencyData = "";
        if (data.currencies) {
          currencyData = Object.values(data.currencies).map((currency) => currency.name).join(", ");
        }

        tableBody.innerHTML = `
          <tr>
            <td style="max-width: 100px;">${data.ccn3}</td>
            <td>${selectedCountry}</td>
            <td>${data.capital}</td>
            <td>${data.population}</td>
            <td>${currencyData}</td>
            <td><img src="${data.flags.png}" alt="Flag" /></td>
          </tr>
        `;
      } else {
        tableBody.innerHTML = ""; // Clear the table body if no country is selected
      }
    }
    dropdown.onchange = updateTable; 

  })
  .catch((err) => {
    console.error('Error:', err);
  });
