import { Scalars } from "../../../__generated__/__types__";
import { useHygrothermalDataQuery } from "../../../queries/data.graphql";
import { Skeleton, Result } from "antd";
import { useEffect } from "react";
import { messageApolloError } from "../../../lib/apollo";
import DataPageHeader from "../DataPageHeader";

export type HygrothermalDataProps = {
  hygrothermalDataId: Scalars["Uuid"];
};

export default function HygrothermalData({ hygrothermalDataId }: HygrothermalDataProps) {
  const { loading, error, data } = useHygrothermalDataQuery({
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
