enum PetSpecies {
  dog,
  cat,
  bird,
  rabbit,
  axolotl,
  cachalotinho,
  other;

  static const _toBackend = {
    PetSpecies.dog: 1,
    PetSpecies.cat: 2,
    PetSpecies.bird: 3,
    PetSpecies.rabbit: 4,
    PetSpecies.axolotl: 5,
    PetSpecies.cachalotinho: 6,
    PetSpecies.other: 99,
  };

  static const _fromBackend = {
    1: PetSpecies.dog,
    2: PetSpecies.cat,
    3: PetSpecies.bird,
    4: PetSpecies.rabbit,
    5: PetSpecies.axolotl,
    6: PetSpecies.cachalotinho,
    99: PetSpecies.other,
  };

  int toBackendValue() => _toBackend[this]!;

  static PetSpecies fromBackendValue(int value) =>
      _fromBackend[value] ?? PetSpecies.other;

  String get displayName {
    switch (this) {
      case PetSpecies.dog:
        return 'Dog';
      case PetSpecies.cat:
        return 'Cat';
      case PetSpecies.bird:
        return 'Bird';
      case PetSpecies.rabbit:
        return 'Rabbit';
      case PetSpecies.axolotl:
        return 'Axolotl';
      case PetSpecies.cachalotinho:
        return 'Cachalotinho';
      case PetSpecies.other:
        return 'Other';
    }
  }
}
