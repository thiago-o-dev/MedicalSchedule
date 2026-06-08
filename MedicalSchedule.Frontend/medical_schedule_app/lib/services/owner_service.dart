import 'package:dio/dio.dart';

import '../core/network/api_client.dart';
import '../models/owner/owner_model.dart';

class OwnerService {
  final Dio _dio = ApiClient.dio;

  Future<Response> createOwner(OwnerModel owner) async {
    return await _dio.post('/api/owners', data: owner.toJson());
  }

  Future<Response> getOwners() async {
    return await _dio.get('/api/owners');
  }

  Future<Response> getOwnerById(String id) async {
    return await _dio.get('/api/owners/$id');
  }
}
