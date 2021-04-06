import { Component, Input, OnInit } from '@angular/core';
import { FileModelCharacteristics } from 'src/app/models/models.model';

@Component({
  selector: 'app-prices',
  templateUrl: './prices.component.html',
  styleUrls: ['./prices.component.css']
})
export class PricesComponent implements OnInit {

	@Input() fileModelCharacteristics: FileModelCharacteristics = new FileModelCharacteristics();

	constructor() { }
	ngOnInit(): void {}


}
