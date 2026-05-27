import {
  FloatPropositionInput,
  IntPropositionInput,
  StringPropositionInput,
  UrlPropositionInput,
  UuidPropositionInput,
} from "../__generated__/graphql";

type DistributiveOmit<T, K extends keyof any> = T extends any
  ? Omit<T, K>
  : never;

type Scalar = string | number | boolean;

export type EnumPropositionInput<TEnum> = {
  equalTo?: TEnum | null;
  in?: TEnum[] | null;
  notEqualTo?: TEnum | null;
  notIn?: TEnum[] | null;
};

export type ListPropositionInput<TItemPropositionInput = any> = {
  all?: TItemPropositionInput | null;
  none?: TItemPropositionInput | null;
  some?: TItemPropositionInput | null;
};

type FilterOperatorKind = "single" | "multiple";

const ScalarFilterOperatorMap = {
  contains: { label: "contains", kind: "single" },
  startsWith: { label: "starts with", kind: "single" },
  endsWith: { label: "ends with", kind: "single" },
  equalTo: { label: "=", kind: "single" },
  notEqualTo: { label: "≠", kind: "single" },
  greaterThan: { label: ">", kind: "single" },
  greaterThanOrEqualTo: { label: "≥", kind: "single" },
  lessThan: { label: "<", kind: "single" },
  lessThanOrEqualTo: { label: "≤", kind: "single" },
  in: { label: "∈", kind: "multiple" },
  notIn: { label: "∉", kind: "multiple" },
} as const satisfies Record<
  string,
  { label: string; kind: FilterOperatorKind }
>;

const ListFilterOperatorMap = {
  all: { label: "all", kind: "single" },
  none: { label: "none", kind: "single" },
  some: { label: "some", kind: "single" },
} as const satisfies Record<
  string,
  { label: string; kind: FilterOperatorKind }
>;

const FilterOperatorMap = {
  ...ScalarFilterOperatorMap,
  ...ListFilterOperatorMap,
} as const;

type ScalarFilterOperator = keyof typeof ScalarFilterOperatorMap;
type ListFilterOperator = keyof typeof ListFilterOperatorMap;
export type FilterOperator = ScalarFilterOperator | ListFilterOperator;

export const getFilterOperatorLabel = (key: FilterOperator) =>
  FilterOperatorMap[key].label;
export const isFilterOperatorSingle = (key: FilterOperator) =>
  FilterOperatorMap[key].kind === "single";

const scalarFilterOperators = new Set(
  Object.keys(ScalarFilterOperatorMap),
) as ReadonlySet<ScalarFilterOperator>;
const listFilterOperators = new Set(
  Object.keys(ListFilterOperatorMap),
) as ReadonlySet<ListFilterOperator>;

const isScalarFilterOperator = (
  value: FilterOperator | undefined,
): value is ScalarFilterOperator => {
  return scalarFilterOperators.has(value as ScalarFilterOperator);
};
const isListFilterOperator = (
  value: FilterOperator | undefined,
): value is ListFilterOperator => {
  return listFilterOperators.has(value as ListFilterOperator);
};

const defineFilterOperators = <K extends FilterOperator>(
  keys: readonly K[],
): readonly K[] => keys;

type ScalarFilterState = {
  readonly operator: ScalarFilterOperator;
  readonly value: Scalar | Scalar[] | null;
};
type ListFilterState = {
  readonly operator: ListFilterOperator;
  readonly value: FilterState;
};
export type ObjectFilterState = {
  readonly operator?: undefined;
  readonly index: number;
  readonly value: FilterState;
};

type FilterState = ScalarFilterState | ListFilterState | ObjectFilterState;

const ScalarFilterTypeMap = {
  enum: {
    operators: defineFilterOperators(["equalTo", "notEqualTo", "in", "notIn"]),
    initialOperator: 0,
  },
  float: {
    operators: defineFilterOperators([
      "equalTo",
      "notEqualTo",
      "greaterThan",
      "greaterThanOrEqualTo",
      "lessThan",
      "lessThanOrEqualTo",
      "in",
      "notIn",
    ]),
    initialOperator: 0,
  },
  int: {
    operators: defineFilterOperators([
      "equalTo",
      "notEqualTo",
      "greaterThan",
      "greaterThanOrEqualTo",
      "lessThan",
      "lessThanOrEqualTo",
      "in",
      "notIn",
    ]),
    initialOperator: 0,
  },
  string: {
    operators: defineFilterOperators([
      "contains",
      "startsWith",
      "endsWith",
      "equalTo",
      "notEqualTo",
      "in",
      "notIn",
    ]),
    initialOperator: 0,
  },
  url: {
    operators: defineFilterOperators(["equalTo", "notEqualTo", "in", "notIn"]),
    initialOperator: 0,
  },
  uuid: {
    operators: defineFilterOperators(["equalTo", "notEqualTo", "in", "notIn"]),
    initialOperator: 0,
  },
} as const satisfies Record<
  string,
  { operators: readonly ScalarFilterOperator[]; initialOperator: number }
>;

const ListFilterTypeMap = {
  list: {
    operators: defineFilterOperators(["some", "all", "none"]),
    initialOperator: 0,
  },
} as const satisfies Record<
  string,
  { operators: readonly ListFilterOperator[]; initialOperator: number }
>;

export const FilterTypeMap = {
  ...ScalarFilterTypeMap,
  ...ListFilterTypeMap,
} as const;

type ScalarFilterType = keyof typeof ScalarFilterTypeMap;
type ListFilterType = keyof typeof ListFilterTypeMap;
type ObjectFilterType = typeof objectFilterType;
type FilterType = ScalarFilterType | ListFilterType | ObjectFilterType;

const scalarFilterTypes = new Set(
  Object.keys(ScalarFilterTypeMap),
) as ReadonlySet<ScalarFilterType>;
const listFilterTypes = new Set(
  Object.keys(ListFilterTypeMap),
) as ReadonlySet<ListFilterType>;
const objectFilterType = "object" as const;

const isScalarFilterType = (value: FilterType): value is ScalarFilterType => {
  return scalarFilterTypes.has(value as ScalarFilterType);
};
const isListFilterType = (value: FilterType): value is ListFilterType => {
  return listFilterTypes.has(value as ListFilterType);
};

type BaseFilterDefinition<TKey> = {
  readonly field: TKey;
  readonly label?: string;
};

type EnumFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
  TEnum,
> = BaseFilterDefinition<TKey> & {
  readonly type: "enum" & ScalarFilterType;
  readonly field: TKey;
  readonly enumObject: TEnum;
  readonly _validation?: TPropositionInput[TKey] extends EnumPropositionInput<TEnum>
    ? TKey
    : never;
};

type FloatFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "float" & ScalarFilterType;
  readonly _validation?: TPropositionInput[TKey] extends FloatPropositionInput
    ? TKey
    : never;
};

type IntFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "int" & ScalarFilterType;
  readonly _validation?: TPropositionInput[TKey] extends IntPropositionInput
    ? TKey
    : never;
};

type StringFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "string" & ScalarFilterType;
  readonly _validation?: TPropositionInput[TKey] extends StringPropositionInput
    ? TKey
    : never;
};

type UrlFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "url" & ScalarFilterType;
  readonly _validation?: TPropositionInput[TKey] extends UrlPropositionInput
    ? TKey
    : never;
};

type UuidFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "uuid" & ScalarFilterType;
  readonly _validation?: TPropositionInput[TKey] extends UuidPropositionInput
    ? TKey
    : never;
};

type ScalarFilterDefinitionMap<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = {
  enum: TPropositionInput[TKey] extends EnumPropositionInput<infer TEnum>
    ? EnumFilterDefinition<TPropositionInput, TKey, TEnum>
    : never;
  float: FloatFilterDefinition<TPropositionInput, TKey>;
  int: IntFilterDefinition<TPropositionInput, TKey>;
  string: StringFilterDefinition<TPropositionInput, TKey>;
  url: UrlFilterDefinition<TPropositionInput, TKey>;
  uuid: UuidFilterDefinition<TPropositionInput, TKey>;
};

type ScalarFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = ScalarFilterDefinitionMap<TPropositionInput, TKey>[ScalarFilterType];

type ListItemFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> =
  NonNullable<TPropositionInput[TKey]> extends {
    all?: infer TItemPropositionInput;
  }
    ? DistributiveOmit<
        FilterDefinition<NonNullable<TItemPropositionInput>>,
        "field"
      >
    : never;

export type ListFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "list" & ListFilterType;
  readonly item: ListItemFilterDefinition<TPropositionInput, TKey>;
  readonly _validation?: TPropositionInput extends ListPropositionInput
    ? TKey
    : never;
};

export type ObjectFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> = BaseFilterDefinition<TKey> & {
  readonly type: "object" & ObjectFilterType;
  readonly items: readonly FilterDefinition<
    NonNullable<TPropositionInput[TKey]>
  >[];
};

type FieldFilterDefinition<
  TPropositionInput,
  TKey extends keyof TPropositionInput,
> =
  | ScalarFilterDefinition<TPropositionInput, TKey>
  | ListFilterDefinition<TPropositionInput, TKey>
  | ObjectFilterDefinition<TPropositionInput, TKey>;

type AnyFilterDefinition = FieldFilterDefinition<any, any>;

export type FilterDefinition<TPropositionInput> = {
  [TKey in keyof TPropositionInput]: FieldFilterDefinition<
    TPropositionInput,
    TKey
  >;
}[keyof TPropositionInput];

const isScalarFilterDefinition = <TPropositionInput>(
  value: FilterDefinition<TPropositionInput>,
): value is ScalarFilterDefinition<
  TPropositionInput,
  keyof TPropositionInput
> => {
  return isScalarFilterType(value.type);
};
const isListFilterDefinition = <TPropositionInput>(
  value: FilterDefinition<TPropositionInput>,
): value is ListFilterDefinition<
  TPropositionInput,
  keyof TPropositionInput
> => {
  return isListFilterType(value.type);
};
const isObjectFilterDefinition = <TPropositionInput>(
  value: FilterDefinition<TPropositionInput>,
): value is ObjectFilterDefinition<
  TPropositionInput,
  keyof TPropositionInput
> => {
  return value.type === objectFilterType;
};

const isScalarFilterState = (
  value: FilterState,
): value is ScalarFilterState => {
  return isScalarFilterOperator(value.operator);
};
const isListFilterState = (value: FilterState): value is ListFilterState => {
  return isListFilterOperator(value.operator);
};
const isObjectFilterState = (
  value: FilterState,
): value is ObjectFilterState => {
  return value.operator === undefined;
};

export type FilterStateReducerContext = readonly AnyFilterDefinition[];

export const createFilterStateReducer = <TOutput>(
  reduceScalar: (
    scalar: ScalarFilterState,
    context: FilterStateReducerContext,
  ) => TOutput,
  reduceList: (
    list: ListFilterState,
    reduce: (filter: FilterState) => TOutput,
  ) => TOutput,
  reduceObject: (
    object: ObjectFilterState,
    context: FilterStateReducerContext,
    reduce: (filter: FilterState) => TOutput,
  ) => TOutput,
) => {
  return (
    root: ObjectFilterState,
    initialContext: FilterStateReducerContext,
  ): TOutput => {
    const reduce = (
      filter: FilterState,
      context: FilterStateReducerContext,
    ): TOutput => {
      if (isScalarFilterState(filter)) {
        return reduceScalar(filter, context);
      }
      if (isListFilterState(filter)) {
        return reduceList(filter, (x) => reduce(x, context));
      }
      if (isObjectFilterState(filter)) {
        const subContext = (
          context[filter.index] as ObjectFilterDefinition<any, keyof any>
        ).items;
        return reduceObject(filter, context, (x) => reduce(x, subContext));
      }
      return assertNever(filter);
    };
    return reduceObject(root, initialContext, (object) =>
      reduce(object, initialContext),
    );
  };
};

export const createFilterDefinitionReducer = <
  TScalarOutput,
  TListOutput,
  TObjectOutput,
>(
  reduceScalar: (scalar: ScalarFilterDefinition<any, any>) => TScalarOutput,
  reduceList: (
    list: ListFilterDefinition<any, any>,
    reduce: (
      filter: AnyFilterDefinition,
    ) => TScalarOutput | TListOutput | TObjectOutput,
  ) => TListOutput,
  reduceObject: (
    object: ObjectFilterDefinition<any, any>,
    reduce: (
      filter: AnyFilterDefinition,
    ) => TScalarOutput | TListOutput | TObjectOutput,
  ) => TObjectOutput,
) => {
  return (
    root: AnyFilterDefinition,
  ): TScalarOutput | TListOutput | TObjectOutput => {
    const reduce = (
      filter: AnyFilterDefinition,
    ): TScalarOutput | TListOutput | TObjectOutput => {
      if (isScalarFilterDefinition(filter)) {
        return reduceScalar(filter);
      }
      if (isListFilterDefinition(filter)) {
        return reduceList(filter, (x) => reduce(x));
      }
      if (isObjectFilterDefinition(filter)) {
        return reduceObject(filter, (x) => reduce(x));
      }
      return null as TScalarOutput | TListOutput | TObjectOutput;
      // TODO make the following succeed: return assertNever(filter);
    };
    return reduce(root);
  };
};

// to GraphQL filter input, for example, to `{ name: { contains: "Simon" } }`
export const toWhereClause = <TPropositionInput>(
  filter: ObjectFilterState,
  configs: readonly FilterDefinition<TPropositionInput>[],
): TPropositionInput =>
  // the initial context type is `readonly FilterDefinition<TPropositionInput>[]` and subsequent
  // types are of the form `readonly FilterDefinition<TPropositionInput[keyof TPropositionInput]>[]`
  createFilterStateReducer<any>(
    (scalar, _) => ({
      [scalar.operator]: scalar.value,
    }),
    (list, reduce) => ({
      [list.operator]: reduce(list.value),
    }),
    (object, context, reduce) => ({
      [context[object.index].field]: reduce(object.value),
    }),
  )(filter, configs as FilterStateReducerContext) as TPropositionInput;

export const getInitialFilterSubformValues = (
  filterDefinition: AnyFilterDefinition,
): FilterState =>
  createFilterDefinitionReducer<
    ScalarFilterState,
    ListFilterState,
    ObjectFilterState
  >(
    (scalar) => ({
      operator:
        ScalarFilterTypeMap[scalar.type].operators[
          ScalarFilterTypeMap[scalar.type].initialOperator
        ],
      value: null,
    }),
    (list, reduce) => ({
      operator:
        ListFilterTypeMap[list.type].operators[
          ListFilterTypeMap[list.type].initialOperator
        ],
      value: reduce(list.item),
    }),
    (object, reduce) => ({
      index: 0,
      value: reduce(object.items[0]),
    }),
  )(filterDefinition);
