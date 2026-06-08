import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../models/vet/vet_model.dart';

class VetService {
  final Dio _dio = ApiClient.dio;

  Future<Response> createVet(VetModel vet) async {
    return await _dio.post('/api/vets', data: vet.toJson());
  }

  Future<Response> getVets() async {
    return await _dio.get('/api/vets');
  }

  Future<Response> getVetById(String id) async {
    return await _dio.get('/api/vets/$id');
  }
}
