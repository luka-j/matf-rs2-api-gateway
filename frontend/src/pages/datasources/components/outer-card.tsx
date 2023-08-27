import React, { useState } from "react";
import { PlusCircle } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

import AddDatasourceForm from "./add-datasource-form";

interface IOuterCardProps {
  datasourceType: string;
  children: React.ReactNode;
}

const OuterCard = ({ datasourceType, children }: IOuterCardProps) => {
  const [openDialog, setOpenDialog] = useState(false);

  return (
    <Card className="mx-4">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          {datasourceType}
          <Dialog open={openDialog} onOpenChange={setOpenDialog}>
            <DialogTrigger asChild>
              <Button variant="ghost" size="icon" className="mt-1">
                <PlusCircle />
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle className="text-center text-xl">
                  Add a {datasourceType.slice(0, -1)}
                </DialogTitle>
                <DialogDescription>
                  Fill the form below to add a {datasourceType.slice(0, -1)}:
                </DialogDescription>
              </DialogHeader>
              <AddDatasourceForm
                setOpenDialog={setOpenDialog}
                datasourceType={datasourceType.slice(0, -1).toLowerCase()}
              />
            </DialogContent>
          </Dialog>
        </CardTitle>
        <CardDescription>
          Here you can preview and edit your {datasourceType.toLowerCase()}.
        </CardDescription>
      </CardHeader>

      <CardContent className="flex flex-col items-center justify-between gap-8 md:flex-row">
        {children}
      </CardContent>
    </Card>
  );
};

export default OuterCard;
