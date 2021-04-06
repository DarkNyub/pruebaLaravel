import { Injectable } from '@angular/core';
import { HttpClient,HttpHeaders } from '@angular/common/http';
import { FileModelCharacteristics, FileModelOffer } from '../models/models.model';

@Injectable({
	providedIn: 'root'
})
export class ReadDataService {

	constructor(private http:HttpClient) { }
	//Traer todas las categorias de la plataforma
	getFileData(){
		return this.http.get('../../assets/files/ofertas.json');
	}
	getFileVersions(pidoffer:string){
		var pFileModelOffer: FileModelOffer[] = [];
		this.http.get('../../assets/files/ofertas.json').subscribe((data:any)=>(
			pFileModelOffer = data
		))
		return pFileModelOffer.find(d => d.id == pidoffer)?.versions[0];
	}
}