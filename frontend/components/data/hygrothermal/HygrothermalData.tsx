import { Scalars } from "../../../__generated__/graphql";
import { HygrothermalDataDocument } from "../../../queries/data.generated";
import { Skeleton, Result } from "antd";
import { useEffect } from "react";
import { messageApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";
import { useQuery } from "@apollo/client/react";

export type HygrothermalDataProps = {
  hygrothermalDataId: Scalars["Uuid"]["input"];
};

export default function HygrothermalData({ hygrothermalDataId }: HygrothermalDataProps) {
  const { loading, error, data } = useQuery(HygrothermalDataDocument, {
    variables: {
      uuid: hygrothermalDataId,
    },
  });
  const hygrothermalData = data?.hygrothermalData;

  useEffect(() => {
    if (error) {
      messageApolloError(error);
    }
  }, [error]);

  if (loading) {
    return <Skeleton active avatar title />;
  }

  if (!hygrothermalData) {
    return (
      <Result
        status="500"
        title="500"
        subTitle="Sorry, something went wrong."
      />
    );
  }

  return (
    <DataPageHeader
      data={hygrothermalData}
    // extra={[
    //   <UpdateHygrothermalData
    //     key="updateHygrothermalData"
    //     hygrothermalDataId={hygrothermalData.uuid}
    //   />,
    // ]}
    >
    </DataPageHeader>
  );
}
